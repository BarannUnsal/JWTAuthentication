using JWTLogin.BL.Abstract;
using JWTLogin.BL.Concrete;
using JWTLogin.DAL.Contexts;
using JWTLogin.Entity.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace JWTLogin.BL
{
    public static class ServiceRegistration
    {
        public static IConfiguration Configuration { get; }
        public static void AddBLService(this IServiceCollection services)
        {
            services.AddScoped<IUsersService, UsersManager>();
        }

        public static void AddAuthenticationService(this IServiceCollection services)
        {
            string secretStr = Configuration["Jwt:Secret"];
            byte[] secret = Encoding.UTF8.GetBytes(secretStr);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                //http paketi gönderiliriken meta dataya ihtiyaç yok
                options.RequireHttpsMetadata = false;
                options.Audience = Configuration["Jwt:Audience"];
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(secret),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero

                };
            });
        }
        public static void AddIdentityService(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters =
                "abcçdefgğhijklmnopqrstuüvwxyzABCÇDEFGĞHIİJKLMNOÖPQRSTÜUVWXYZ";
                options.Password.RequiredLength = 3;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<JwtLoginDbContext>().AddDefaultTokenProviders();
        }
    }
}
