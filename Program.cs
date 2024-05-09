using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PatrikFunctionApp.DataAcress;


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {

        var keyvaultEndPoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
        config.AddAzureKeyVault(keyvaultEndPoint, new DefaultAzureCredential());

    })
    .ConfigureServices(async (hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        string SQLConnection = configuration["testtesttest"];

        //spara för localhost testning

        // var configBuilder = new ConfigurationBuilder()
        //     .SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
        //     .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
        //     .AddEnvironmentVariables();
        //

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(SQLConnection);
        });


        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

await host.RunAsync();