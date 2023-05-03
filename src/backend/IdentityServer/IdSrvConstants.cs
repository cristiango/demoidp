using Duende.IdentityServer.Models;

namespace demoidp.IdentityServer;

public static class IdSrvConstants
{
    public const string RoleClaimType = "role";
    
    public class Role : IdentityResource
    {
        public Role() : base(RoleClaimType, new []{ RoleClaimType }) {}
    }
}