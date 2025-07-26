using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure.Extensions;

public static class DbContextRegistration
{
    public static IServiceCollection AddModuleDbContext<TContext>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TContext : DbContext
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<TContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
