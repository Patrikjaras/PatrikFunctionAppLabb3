using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PatrikFunctionApp.DataAcress;
using PatrikFunctionApp.Model;

namespace PatrikFunctionApp.Functions
{
    public class ProductsGetAllCreate
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductsGetAllCreate> _logger;
        public ProductsGetAllCreate(AppDbContext context, ILogger<ProductsGetAllCreate> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Function("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts(
            [HttpTrigger(AuthorizationLevel.Anonymous,  "get", Route = "products")] HttpRequest req)
        {
            
            var products = await _context.Products.ToListAsync();
            _logger.LogInformation("Get all products");
            return new OkObjectResult(products);
        }

        [Function("CreateProduct")]
        public async Task<IActionResult> CreateProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products")] HttpRequest req)
        {
            

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var product = JsonConvert.DeserializeObject<Product>(requestBody);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created product");
            return new CreatedResult("/products", product);

        }
       
        [Function("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "products/{id}")] HttpRequest req, Guid id)
        {
            _logger.LogInformation("Deletes a product");
            var productToDelete = await _context.Products.FirstOrDefaultAsync(product => product.Id == id);
       
            if (productToDelete == null)
            {
                return new NotFoundResult();
            }
            else
            {
                 _context.Products.Remove(productToDelete);
                 await _context.SaveChangesAsync();
                return new OkObjectResult("Item was deleted");
            }
       
        }
       
        [Function("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "products/{id}")] HttpRequest req, Guid id)
        {
          
       
           var prodcutToUpdate = await _context.Products.FirstOrDefaultAsync(product => product.Id == id);
       
            if (prodcutToUpdate == null)
            {
                return new NotFoundResult();
            }
            else
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var newProductInformation = JsonConvert.DeserializeObject<Product>(requestBody);
                prodcutToUpdate.Name = newProductInformation.Name;
                prodcutToUpdate.Price  = newProductInformation.Price;
                _context.Products.Update(prodcutToUpdate);
                await _context.SaveChangesAsync();
                return new OkObjectResult(prodcutToUpdate);
            }
        }
       
        [Function("GetProduct")]
        public async Task<IActionResult> GetProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products/{id}")] HttpClient req, Guid id)
        {
            var selectedProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (selectedProduct == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(selectedProduct);
            }
       
       
       
        }
    }
}

