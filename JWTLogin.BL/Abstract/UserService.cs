using JWTLogin.BL.DTOs;
using JWTLogin.Entity.Identity;

namespace JWTLogin.BL.Abstract
{
    public interface IUsersService
    {
        Task<string> SignInAsync(UserDTO userDTO);
        Task<string> LoginAsync(LoginDTO loginDTO);
    }
}
