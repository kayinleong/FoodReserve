using FoodReserve.API.Services;
using FoodReserve.SharedLibrary.Constants;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodReserve.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Superuser,Admin")]
    [Route("api/[controller]")]
    public class CustomerController(UserService userService, CustomerService customerService) : ControllerBase
    {
        [HttpGet]
        public PaginatedResponse<IEnumerable<CustomerResponse>> GetAll(int pageNumber, int pageSize, string? keyword)
        {
            var data = customerService.GetAll(pageNumber, pageSize, keyword);

            return new()
            {
                Response = data,
                CurrentPage = data.CurrentPage,
                TotalPages = data.TotalPages,
                TotalCount = data.TotalCount
            };
        }

        [HttpGet("user")]
        public PaginatedResponse<IEnumerable<UserResponse>> GetAllUser(int pageNumber, int pageSize, string? keyword)
        {
            var data = userService.GetAll(pageNumber, pageSize, UserRole.Guest, keyword);

            return new()
            {
                Response = data,
                CurrentPage = data.CurrentPage,
                TotalPages = data.TotalPages,
                TotalCount = data.TotalCount
            };
        }

        [HttpGet("{id}")]
        public async Task<CustomerResponse> GetByIdAsync(string id)
        {
            return (CustomerResponse)await customerService.GetByIdAsync(id);
        }

        [HttpGet("user/{id}")]
        public async Task<UserResponse> GetUserByIdAsync(string id)
        {
            return (UserResponse)await userService.GetByIdAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CustomerRequest data)
        {
            await customerService.CreateAsync(data);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, CustomerRequest data)
        {
            await customerService.UpdateByIdAsync(id, data);
            return Ok();
        }

        [HttpPost("toggle/{id}")]
        public async Task<IActionResult> ToggleStatusAsync(string id)
        {
            await customerService.ToggleStatusByIdAsync(id);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await customerService.DeleteByIdAsync(id);
            return Ok();
        }
    }
}
