using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly AppDbContext _ctx;

        public ProductsGetAllCreate(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        [Function("ProductsGetAllCreate")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "products")] HttpRequest req)
        {
            string functionKey = req.Headers["x-functions-key"];
            string expextedKey = Environment.GetEnvironmentVariable("SuperSecretKey");

            if (functionKey != expextedKey)
            {
                return new UnauthorizedResult();
            }

            if (req.Method == HttpMethods.Post)
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var product = JsonConvert.DeserializeObject<Product>(requestBody);
                _ctx.Products.Add(product);
                await _ctx.SaveChangesAsync();
                return new CreatedResult("/products", product);
            }

            var products = await _ctx.Products.ToListAsync();
            return new OkObjectResult(products);
        }
    }
}

