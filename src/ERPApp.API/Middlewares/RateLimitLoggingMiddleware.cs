using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class RateLimitLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitLoggingMiddleware> _logger;

    public RateLimitLoggingMiddleware(RequestDelegate next, ILogger<RateLimitLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Capture status before and after
        await _next(context);

        if (context.Response.StatusCode == 429)
        {
            var clientId = context.Request.Headers["X-ClientId"].ToString();
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "UnknownIP";
            var path = context.Request.Path;

            string logMessage = string.IsNullOrEmpty(clientId)
                ? $"[RateLimit Triggered] IP-based limit exceeded | IP: {ip} | Path: {path}"
                : $"[RateLimit Triggered] Client-based limit exceeded | ClientId: {clientId} | IP: {ip} | Path: {path}";

            _logger.LogWarning(logMessage);
        }
    }
}
