using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaForge.Identity.API.Data;

public static class IdentityServerSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

        if (!await context.Clients.AnyAsync())
        {
            foreach (var client in GetClients())
                context.Clients.Add(client.ToEntity());
        }

        if (!await context.ApiScopes.AnyAsync())
        {
            foreach (var scopeItem in GetApiScopes())
                context.ApiScopes.Add(scopeItem.ToEntity());
        }

        if (!await context.IdentityResources.AnyAsync())
        {
            foreach (var resource in GetIdentityResources())
                context.IdentityResources.Add(resource.ToEntity());
        }

        await context.SaveChangesAsync();
    }

    private static IEnumerable<Client> GetClients() =>
    [
        new()
        {
            ClientId = "mediaforge_gateway",
            ClientSecrets = { new Secret("gateway_secret".Sha256()) },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            AllowedScopes = { "identity.read", "media.read" }
        },
        new()
        {
            ClientId = "mediaforge_spa",
            RequireClientSecret = false,
            AllowedGrantTypes = GrantTypes.Code,
            RequirePkce = true,
            RedirectUris = { "https://localhost:5173/callback" },
            PostLogoutRedirectUris = { "https://localhost:5173" },
            AllowedScopes = { "openid", "profile", "email", "identity.read", "media.read", "search.read" },
            AllowOfflineAccess = true
        }
    ];

    private static IEnumerable<ApiScope> GetApiScopes() =>
    [
        new("identity.read"),
        new("media.read"),
        new("search.read")
    ];

    private static IEnumerable<IdentityResource> GetIdentityResources() =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Email()
    ];
}
