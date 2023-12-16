using JWTAuthentication.Dtos;
using JWTAuthentication.Dtos.AuthDto;

namespace JWTAuthentication.Interfaces.Repos
{
    public interface IAuth
    {
        Task<RegistrationResponse> Registration(RegistrationRequest req);
        Task<LoginResponse> Login(LoginRequest req);
        Task<GetUserInfoByIdResponse> GetUserInfoById(GetUserInfoByIdRequest req);
    }
}
