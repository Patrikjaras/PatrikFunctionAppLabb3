using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PatrikFunctionApp.DataAcress;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(async (hostContext, services) =>
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        IConfiguration configuration = configBuilder.Build();
        var sqlConnectionLive = Environment.GetEnvironmentVariable("SQlConnectionString");

        if (sqlConnectionLive is null)
        {
            string sqlConnectionStringLocal = configuration.GetConnectionString("SQlConnectionString");
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(sqlConnectionStringLocal);
            });

        }
        else
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(sqlConnectionLive);
            });
        }

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

await host.RunAsync();