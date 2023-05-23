using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

namespace demoidp.IdentityServer;

public class DemoProfileService : IProfileService
{
    public Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = context.Subject;
        context.IssuedClaims.AddRange(user.Claims);
        return Task.CompletedTask;
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = true;
        return Task.CompletedTask;
    }
}