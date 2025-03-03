using FoodReserve.API.Models;
using FoodReserve.API.Services;
using FoodReserve.SharedLibrary.Constants;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodReserve.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "SUPERUSER,ADMIN")]
    [Route("api/[controller]")]
    public class StaffController(UserService userService) : ControllerBase
    {
        [HttpGet]
        public PaginatedResponse<IEnumerable<UserResponse>> GetAll(int pageNumber, int pageSize, string? keyword)
        {
            var data = userService.GetAll(pageNumber, pageSize, UserRole.OUTLET, keyword);

            return new()
            {
                Response = data,
                CurrentPage = data.CurrentPage,
                TotalPages = data.TotalPages,
                TotalCount = data.TotalCount
            };
        }

        [HttpGet("{id}")]
        public async Task<UserResponse> GetByIdAsync(string id)
        {
            return await userService.GetByIdAsync(id);
        }

        [HttpGet("username/{username}")]
        public async Task<UserResponse> GetByUsernameAsync(string username)
        {
            return await userService.GetByUsernameAsync(username);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(UserCreateRequest user)
        {
            var data = (User)user;
            data.Role = UserRole.OUTLET;

            await userService.CreateAsync(data);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, UserUpdateRequest user)
        {
            var data = (User)user;
            data.Role = UserRole.OUTLET;

            await userService.UpdateByIdAsync(id, data);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await userService.DeleteByIdAsync(id);
            return Ok();
        }
    }
}
