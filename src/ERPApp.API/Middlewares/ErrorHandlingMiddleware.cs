using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);

            if (!context.Response.HasStarted && context.Response.StatusCode == StatusCodes.Status400BadRequest &&
                context.Items["ModelStateErrors"] is ValidationProblemDetails validationProblem)
            {
                context.Response.ContentType = "application/json";
                var response = JsonSerializer.Serialize(validationProblem);
                await context.Response.WriteAsync(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                message = "An unexpected error occurred.",
                detail = ex.Message
            };

            var json = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(json);
        }
    }
}
