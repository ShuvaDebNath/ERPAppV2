using AspNetCoreRateLimit;
using Auth.Infrastructure;
using Auth.Infrastructure.Extensions;
using ERApp.API.Extensions;
using ERPApp.Shared.Interfaces;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("serilog.json")
        .Build())
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var installers = AppDomain.CurrentDomain
    .GetAssemblies()
    .SelectMany(assembly => assembly.GetTypes())
    .Where(type => typeof(IServiceInstaller).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
    .Select(Activator.CreateInstance)
    .Cast<IServiceInstaller>();

foreach (var installer in installers)
{
    installer.InstallServices(builder.Services, builder.Configuration);
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
        policy =>
        {
            policy.AllowAnyOrigin()
            //WithOrigins("https://your-frontend-url.com")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddModuleDbContext<AuthDbContext>(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IClientResolveContributor, JwtClientResolveContributor>();
builder.Services.AddCustomHealthChecks(builder.Configuration);

builder.Services.AddSharedServices();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSecurityHeaders();
app.UseRouting();
app.UseRequestLogging();
app.UseHttpsRedirection();
app.UseMiddleware<RateLimitLoggingMiddleware>();
app.UseIpRateLimiting();
app.UseClientRateLimiting();
app.UseCors("AllowOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapCustomHealthCheckEndpoints();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers(); 
app.Run();