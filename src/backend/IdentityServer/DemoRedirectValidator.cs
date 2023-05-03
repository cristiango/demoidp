using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;

namespace demoidp.IdentityServer;

public class DemoRedirectValidator : IRedirectUriValidator
{
    public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
    {
        return Task.FromResult(true);
    }

    public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
    {
        return Task.FromResult(true);
    }
}