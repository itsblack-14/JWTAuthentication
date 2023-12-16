using JWTAuthentication.Const;
using JWTAuthentication.Interfaces.Services;
using System.Security.Claims;

namespace JWTAuthentication.Helpers
{
    public class AuthServices : IAuthService
    {
        public bool IsSeller(ClaimsPrincipal user)
        {
            int userType = int.Parse(user.FindFirst(ClaimTypes.Role).Value);
            return (userType == AuthConst.SELLER_USERTYPE_ID);
        }
    }
}
