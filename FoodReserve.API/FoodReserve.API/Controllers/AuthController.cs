using FoodReserve.API.Models;
using FoodReserve.API.Services;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize]
        [HttpGet]
        public async Task<UserResponse> Userinfo()
        {
            var user = await userService.GetByIdAsync(User.FindFirstValue("id")!);

            return user;
        }

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
