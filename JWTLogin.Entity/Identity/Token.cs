using Microsoft.AspNetCore.Identity;

namespace JWTLogin.Entity.Identity
{
    public class Token : IdentityUserToken<string>
    {
        public DateTime Expiration{ get; set; }
    }
}
