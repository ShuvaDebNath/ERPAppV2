using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ERApp.API.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection string is missing in configuration.");


        services.AddHealthChecks()
             .AddCheck("API", () => HealthCheckResult.Healthy("API is running"), tags: new[] { "api" })
             .AddSqlServer(
                 connectionString: connectionString,
                 healthQuery: "SELECT 1;",
                 name: "SQL Server"
             )
            .AddDiskStorageHealthCheck(options =>
            {
                options.AddDrive("C:\\", 100 * 1024 * 1024); // 100 GB threshold
            }, name: "Disk Space")
            .AddPrivateMemoryHealthCheck(512 * 1024 * 1024, "Memory"); // 512MB

        services.AddHealthChecksUI(setup =>
        {
            setup.SetEvaluationTimeInSeconds(10);
            setup.MaximumHistoryEntriesPerEndpoint(60);
        }).AddInMemoryStorage();

        return services;
    }

    public static IEndpointRouteBuilder MapCustomHealthCheckEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health-api", new HealthCheckOptions
        {
            Predicate = check => check.Name == "API",
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        endpoints.MapHealthChecks("/health-sql", new HealthCheckOptions
        {
            Predicate = check => check.Name == "SQL Server",
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        endpoints.MapHealthChecks("/health-disk", new HealthCheckOptions
        {
            Predicate = check => check.Name == "Disk Space",
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        endpoints.MapHealthChecks("/health-memory", new HealthCheckOptions
        {
            Predicate = check => check.Name == "Memory",
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        endpoints.MapHealthChecksUI(options =>
        {
            options.UIPath = "/health-ui";
            options.ApiPath = "/health-ui-api";
        });

        return endpoints;
    }
}
