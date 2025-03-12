using System.Collections.Generic;
using System.Security.Claims;
using UserAuthenticationService.Domain;

namespace UserAuthenticationService.ApplicationService.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user);
        string GenerateRefreshToken(string ipAddress);
        ClaimsPrincipal GetPrincipalFromToken(string token);
        IEnumerable<Claim> GetClaimsFromUser(User user);
    }
} 