using JWTAuthentication.Dtos.AuthDto;
using JWTAuthentication.Helpers;
using JWTAuthentication.Interfaces;
using JWTAuthentication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth _repo;
        private readonly IAuthService _services;

        public AuthController(IAuth repo,IAuthService services)
        {
            _repo = repo;
            _services = services;
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> Registration(RegistrationRequest req)
        {
            try
            {
                var response = await _repo.Registration(req);
                return StatusCode(response.StatusCode, response);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            try
            {
                var response = await _repo.Login(req);
                return StatusCode(response.StatusCode, response);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetUserInfoById")]
        [Authorize]
        public async Task<IActionResult> GetUserInfoById([FromQuery]GetUserInfoByIdRequest req)
        {
            try
            {
                var isSeller = _services.IsSeller(User);

                if(!isSeller)
                {
                    return Unauthorized();
                }

                var response = await _repo.GetUserInfoById(req);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
