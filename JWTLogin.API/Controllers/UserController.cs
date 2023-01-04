using JWTLogin.BL.Abstract;
using JWTLogin.BL.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace JWTLogin.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUsersService _userService;

        public UserController(IUsersService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignIn([FromBody] UserDTO userDTO)
        {
            var result = await _userService.SignInAsync(userDTO);            
            return Ok(result);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _userService.LoginAsync(loginDTO);
            return Ok(result);
        }
    }
}
