using System.Security.Claims;

namespace JWTAuthentication.Interfaces.Services
{
    public interface IAuthService
    {
        bool IsSeller(ClaimsPrincipal user);
    }
}
