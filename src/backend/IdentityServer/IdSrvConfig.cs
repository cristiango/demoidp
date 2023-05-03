using Duende.IdentityServer.Models;

namespace demoidp.IdentityServer;

public class IdSrvConfig
{
    public static IEnumerable<ApiResource> GetApis()
    {
        return new List<ApiResource>()
        {
        };
    }

    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdSrvConstants.Role(),
            new IdentityResource("offline_access", new[] { "offline_access" })
        };
    }

    public static IEnumerable<Client> GetClients() => new List<Client>()
    {
        new Client
        {
            ClientId = "implicit",
            ClientName = "Embedded Idp Client",
            AllowAccessTokensViaBrowser = true,

            RedirectUris = { "https://notused" },
            PostLogoutRedirectUris = { "https://notused" },
            FrontChannelLogoutUri = "http://localhost:5000/signout-idsrv", // for testing identityserver on localhost

            AllowedGrantTypes = GrantTypes.Implicit,
            AllowedScopes = { "openid", "profile", "email", "role", "api" },
            RequireConsent = false
        },
        new Client
        {
            ClientId = "interactive.public",
            ClientName = "Interactive client (Code with PKCE)",
            //AccessTokenLifetime = 200,
            AllowAccessTokensViaBrowser = false,
            //AuthorizationCodeLifetime = 60,
            //IdentityTokenLifetime = 700,

            RedirectUris = { "https://notused" },
            PostLogoutRedirectUris = { "https://notused" },

            RequireClientSecret = false,

            AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
            AllowedScopes = { "openid", "profile", "email", "api", "offline_access", "role" },

            AllowOfflineAccess = true,
            RefreshTokenUsage = TokenUsage.OneTimeOnly,
            RefreshTokenExpiration = TokenExpiration.Sliding
        }
    };
}