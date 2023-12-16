namespace JWTAuthentication.Dtos.AuthDto
{
    public class GetUserInfoByIdResponse : ResponseStatus
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
