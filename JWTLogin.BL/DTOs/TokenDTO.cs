namespace JWTLogin.BL.DTOs
{
    public class TokenDTO
    {
        public string Token { get; set; }
        public DateTime Expire { get; set; }
    }
}
