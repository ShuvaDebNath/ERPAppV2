using ERApp.Shared.Services.Formatting;
using ERPApp.Shared.Abstractions.Cryptography;
using ERPApp.Shared.Abstractions.Formatting;
using ERPApp.Shared.Services.Cryptography;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IFileSizeFormatter, FileSizeFormatter>();
        return services;
    }
}