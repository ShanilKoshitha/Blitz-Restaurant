using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blitz.Services.Identity
{
    public static class SD
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
                
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("Blitz", "Blitz Server"),
                new ApiScope(name:"read", displayName:"Read your Data"),
                new ApiScope(name:"write", displayName:"Write your Data"),
                new ApiScope(name:"delete", displayName:"Delete your Data")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "Client",
                    ClientSecrets = {new Secret("Secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"read","write","profile" }
                },
                new Client
                {
                    ClientId = "Blitz",
                    ClientSecrets = {new Secret("Secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "http://localhost:44382/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:44382/signout-callback-oidc" }, 
                    AllowedScopes = new List<String>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "Blitz"
                    }
                }

            };
    }
}
