using FoodReserve.API.Services;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodReserve.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "SUPERUSER,ADMIN")]
    [Route("api/[controller]")]
    public class OutletStaffController(StaffService staffService) : ControllerBase
    {
        [HttpGet]
        public PaginatedResponse<IEnumerable<StaffResponse>> GetAll(int pageNumber, int pageSize, string? keyword)
        {
            var data = staffService.GetAll(pageNumber, pageSize, keyword);

            return new()
            {
                Response = data,
                CurrentPage = data.CurrentPage,
                TotalPages = data.TotalPages,
                TotalCount = data.TotalCount
            };
        }

        [HttpGet("{id}")]
        public async Task<StaffResponse> GetByIdAsync(string id)
        {
            return await staffService.GetByIdAsync(id);
        }

        [HttpGet("user/{userId}")]
        public async Task<StaffResponse> GetByUserIdAsync(string userId)
        {
            return await staffService.GetByUserIdAsync(userId);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(StaffRequest staff)
        {
            await staffService.CreateAsync(staff, staff.UserId);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, StaffRequest staff)
        {
            await staffService.UpdateByIdAsync(id, staff);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await staffService.DeleteByIdAsync(id);
            return Ok();
        }
    }
}
