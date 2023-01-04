using JWTLogin.BL.Abstract;
using JWTLogin.BL.DTOs;
using JWTLogin.BL.Helpers;
using JWTLogin.DAL.Contexts;
using JWTLogin.Entity.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace JWTLogin.BL.Concrete
{
    public class UsersManager : IUsersService
    {
        readonly UserManager<AppUser> _userManager;
        readonly SignInManager<AppUser> _signInManager;
        readonly JwtLoginDbContext _context;
        private readonly IConfiguration _config;
        public UsersManager(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, JwtLoginDbContext context, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _config = config;
        }

        ResponseDTO responseDTO = new();
        public async Task<string> LoginAsync(LoginDTO loginDTO)
        {
            AppUser appUser = await _userManager.FindByNameAsync(loginDTO.Username);
            if (appUser == null)
            {
                responseDTO.isSuccess = false;
                responseDTO.Message = "Kullanıcı bulunamadı";
                return responseDTO.Message;
            }
            SignInResult signInResult = await _signInManager.PasswordSignInAsync(appUser, loginDTO.Password, false, false);

            if (signInResult.Succeeded == false)
            {
                responseDTO.isSuccess = false;
                responseDTO.Message = "Kullanıcı adı veya şifre hatalı";
                return responseDTO.Message;
            }

            var user = await _context!.Users!.FirstOrDefaultAsync(x => x.Id == appUser.Id);
            if (user != null)
            {
                AppUser loginUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
                TokenHelper tokenHelper = new TokenHelper(_context, loginUser, _config);
                Token userToken = await tokenHelper.GetTokenAsync();

                responseDTO.isSuccess = true;
                responseDTO.Message = "Giriş başarılı \nToken:" + userToken.Value.ToString();
                return responseDTO.Message;
            }
            else
            {
                responseDTO.isSuccess = false;
                responseDTO.Message = "Hata";
                return responseDTO.Message;
            }
        }

        public async Task<string> SignInAsync(UserDTO userDTO)
        {
            try
            {
                AppUser appUser = await _userManager.FindByNameAsync(userDTO.Username);
                if (appUser != null)
                {
                    responseDTO.isSuccess = false;
                    responseDTO.Message = "Kullanıcı var";
                    return responseDTO.Message;
                }

                AppUser newUser = new();

                newUser.Email = userDTO.Email.Trim();
                newUser.UserName = userDTO.Username.Trim();

                IdentityResult result = await _userManager.CreateAsync(newUser, userDTO.Password.Trim());
                if (result.Succeeded)
                {
                    responseDTO.isSuccess = true;
                    responseDTO.Message = "Kullanıcı oluşturuldu.";
                    return responseDTO.Message;
                }
                else
                {
                    responseDTO.isSuccess = false;
                    responseDTO.Message = string.Format("Hata oluştu: {0}", result.Errors.FirstOrDefault().Description);
                    return responseDTO.Message;
                }

            }
            catch (Exception ex)
            {
                responseDTO.isSuccess = false;
                responseDTO.Message = ex.Message;
                return responseDTO.Message;
            }
        }
    }
}
