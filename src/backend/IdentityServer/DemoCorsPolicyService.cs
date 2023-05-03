using Duende.IdentityServer.Services;

namespace demoidp.IdentityServer;

/// <summary>
/// Allows arbitrary CORS origins - only for demo purposes. NEVER USE IN PRODUCTION
/// </summary>
public class DemoCorsPolicyService: ICorsPolicyService
{
    public Task<bool> IsOriginAllowedAsync(string origin)
    {
        return Task.FromResult(true);
    }
}