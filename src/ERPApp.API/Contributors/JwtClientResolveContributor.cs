using AspNetCoreRateLimit;

public class JwtClientResolveContributor : IClientResolveContributor
{
    public Task<string> ResolveClientAsync(HttpContext context)
    {
        var clientId = context.User?.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString();
        return Task.FromResult(clientId ?? "anonymous");
    }
}
