using FoodReserve.API.Models;
using FoodReserve.API.Services;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodReserve.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        IPasswordHasher<User> hasher,
        AuthService authService,
        UserService userService
    ) : ControllerBase
    {
        [HttpPost]
        public async Task<LoginResponse> Login(UserLoginRequest userLoginRequest)
        {
            var user = await userService.GetByUsernameAsync(userLoginRequest.Username);
            if (hasher.VerifyHashedPassword(user, user.PasswordHashed, userLoginRequest.Password) == PasswordVerificationResult.Failed)
            {
                throw new InvalidOperationException("Invalid user credentials");
            }

            var data = authService.Create(user);
            return new()
            {
                Token = data
            };
        }
    }
}
