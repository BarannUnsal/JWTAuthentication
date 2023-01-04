using JWTLogin.BL.DTOs;
using JWTLogin.DAL.Contexts;
using JWTLogin.Entity.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTLogin.BL.Helpers
{
    public class TokenHelper
    {
        private JwtLoginDbContext _context;
        private AppUser _appUser;
        private IConfiguration _configuration;
        public TokenHelper(JwtLoginDbContext context, AppUser appUser, IConfiguration configuration)
        {
            _context = context;
            _appUser = appUser;
            _configuration = configuration;
        }

        public async Task<Token> GetTokenAsync()
        {
            Token userToken;
            TokenDTO tokenDTO;

            //token'ın daha önceden oluşturulup oluşturulmadığını kontrol eden sorgu
            if (_context.Tokens.Count(u => u.UserId == _appUser.Id) > 0)
            {
                //token bilgisi bulma
                userToken = await _context.Tokens.FirstOrDefaultAsync(t => t.UserId == _appUser.Id);

                //süresi dolmuş tokenlar için yeni token oluşturup günceller
                if (userToken.Expiration <= DateTime.Now)
                {
                    tokenDTO = CreateToken();
                    userToken.Expiration = tokenDTO.Expire;
                    userToken.Value = tokenDTO.Token;
                    _context.Tokens.Update(userToken);
                }
            }
            else
            {
                tokenDTO = CreateToken();

                userToken = new Token();

                userToken.UserId = _appUser.Id;
                userToken.LoginProvider = "Test";
                userToken.Name = _appUser.UserName;
                userToken.Expiration = tokenDTO.Expire;
                userToken.Value = tokenDTO.Token;

                _context.Tokens.Add(userToken);
            }
            await _context.SaveChangesAsync();

            return userToken;

        }

        private TokenDTO CreateToken()
        {
            DateTime expire = DateTime.Now.AddSeconds(30);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"],
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, _appUser.Id),
                    new Claim(ClaimTypes.Name, _appUser.UserName),
                    new Claim(ClaimTypes.Email, _appUser.Email)
                }),
                Expires = expire,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            TokenDTO tokenDTO = new()
            {
                Token = tokenString,
                Expire = expire
            
            };
            return tokenDTO;
        }
    }
}
