using JWTAuthentication.Context;
using JWTAuthentication.Dtos.AuthDto;
using JWTAuthentication.Interfaces.Repos;
using JWTAuthentication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWTAuthentication.Repos
{
    public class AuthRepository : IAuth
    {
        private readonly AuthContext _context;
        private readonly IConfiguration _configuration;

        public AuthRepository(AuthContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<RegistrationResponse> Registration(RegistrationRequest request)
        {
            try
            {
                //User Validation
                var user = await _context.User.AnyAsync(x=>x.IsActive == true && x.Email == request.Email);

                if(user == true)
                {
                    return new RegistrationResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Email has already Registered"
                    };
                }

                CreatePasswordHash(request.Password,out byte[] passwordHash, out byte[] passwordSalt);

                User userToAdd = new User
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = Guid.NewGuid(), //Temporary
                    IsActive = true
                };
                await _context.User.AddAsync(userToAdd);
                await _context.SaveChangesAsync();

                userToAdd.CreatedBy = userToAdd.Id;
                await _context.SaveChangesAsync();

                var token = CreateToken(userToAdd);

                return new RegistrationResponse
                {
                    UserId = userToAdd.Id,
                    Token = token,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success"
                };

            }
            catch(Exception ex)
            {
                return new RegistrationResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                };
            }
        }

        public async Task<LoginResponse> Login(LoginRequest req)
        {
            try
            {
                var User = await _context.User.Where(x=>x.IsActive == true && x.Email == req.Email).FirstOrDefaultAsync();
                if(User == null)
                {
                    return new LoginResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Wrong Email"
                    };
                }

                var isAuthorize = VerifyPasswordHash(req.Password, User.PasswordHash, User.PasswordSalt);
                if(!isAuthorize)
                {
                    return new LoginResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Wrong Password"
                    };
                }

                return new LoginResponse
                {
                    UserId = User.Id,
                    Token = CreateToken(User),
                    Name = User.Name,
                    Email = User.Email,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success"
                };

            }
            catch(Exception e)
            {
                return new LoginResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = e.Message
                };
            }
        }

        public async Task<GetUserInfoByIdResponse> GetUserInfoById(GetUserInfoByIdRequest req)
        {
            try
            {
                var user = await _context.User.Where(x => x.IsActive == true && x.Id == req.Id).FirstOrDefaultAsync();
                if(user == null)
                {
                    return new GetUserInfoByIdResponse
                    {
                        StatusCode = StatusCodes.Status204NoContent,
                        Message = "No User Found"
                    };
                }

                return new GetUserInfoByIdResponse
                {
                    Id = user.Id,
                    UserName = user.Name,
                    Email = user.Email,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success"
                };
            }
            catch(Exception e)
            {
                return new GetUserInfoByIdResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = e.Message
                };
            }
        }

        #region Private Methods
            
        private void CreatePasswordHash(string password,out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA256())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA256(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:SecrectKey").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _configuration.GetSection("AppSettings:Issuer").Value,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var jwt = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return token;
        }

        #endregion
    }
}
