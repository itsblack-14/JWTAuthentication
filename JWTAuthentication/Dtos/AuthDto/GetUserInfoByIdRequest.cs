using System.ComponentModel.DataAnnotations;

namespace JWTAuthentication.Dtos.AuthDto
{
    public class GetUserInfoByIdRequest
    {
        [Required]
        public Guid Id { get; set; }
    }
}
