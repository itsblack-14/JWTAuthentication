using JWTAuthentication.Const;
using JWTAuthentication.Dtos.AuthDto;
using JWTAuthentication.Helpers;
using JWTAuthentication.Interfaces.Repos;
using JWTAuthentication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JWTAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth _repo;
        private readonly IRoleAuthorizationService _services;

        public AuthController(IAuth repo,IRoleAuthorizationService services)
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
                //Authorization
                var roleId = int.Parse(User.FindFirst(ClaimTypes.Role).Value);
                bool isAuthorize = await _services.IsAuthorized(roleId, AuthConst.MODULE_NAME_USERACCOUNT, AuthConst.ACTION_NAME_VIEW);
                if (!isAuthorize)
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
