using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace SchoolManagement.Api.Authentication
{
    public class KeycloakRolesClaimsTransformation : IClaimsTransformation
    {
        private const string RealmAccessClaim = "realm_access";

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identity as ClaimsIdentity;

            if (identity is null || !identity.IsAuthenticated)
                return Task.FromResult(principal);

            var realmAccess = principal.FindFirst(RealmAccessClaim)?.Value;

            if (string.IsNullOrWhiteSpace(realmAccess))
                return Task.FromResult(principal);

            try
            {
                using var document = JsonDocument.Parse(realmAccess);

                if (!document.RootElement.TryGetProperty("roles", out var roles))
                    return Task.FromResult(principal);

                foreach (var role in roles.EnumerateArray())
                {
                    var roleName = role.GetString();

                    if (string.IsNullOrWhiteSpace(roleName)) continue;
                    if (identity.HasClaim(ClaimTypes.Role, roleName)) continue;

                    identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                }
            }
            catch (JsonException)
            {
                return Task.FromResult(principal);
            }

            return Task.FromResult(principal);
        }
    }
}
