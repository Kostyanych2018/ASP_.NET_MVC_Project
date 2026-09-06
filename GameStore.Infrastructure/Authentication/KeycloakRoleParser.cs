using System.Security.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace GameStore.Infrastructure.Authentication;

public static class KeycloakRoleParser
{
    private const string RealmAccessClaimType = "realm_access";
    private const string RolesPropertyName = "roles";

    public static void AddKeycloakRoles(this ClaimsIdentity? identity)
    {
        if (identity == null)
        {
            return;
        }

        var realmAccessClaim = identity.FindFirst(RealmAccessClaimType);
        if (string.IsNullOrWhiteSpace(realmAccessClaim?.Value))
        {
            return;
        }

        try
        {
            using var document = JsonDocument.Parse(realmAccessClaim.Value);
            if (document.RootElement.TryGetProperty(RolesPropertyName, out var rolesElement) &&
                rolesElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var role in rolesElement.EnumerateArray())
                {
                    var roleName = role.GetString();
                    if (!string.IsNullOrWhiteSpace(roleName) && !identity.HasClaim(ClaimTypes.Role, roleName))
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                    }
                }
            }
        }
        catch (JsonException ex)
        {
            throw new AuthenticationException($"Failed to parse Keycloak roles: claim '{RealmAccessClaimType}' contains invalid JSON.", ex);
        }
    }
}