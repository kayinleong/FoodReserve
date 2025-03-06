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
    public class OutletController(OutletService outletService) : ControllerBase
    {
        [HttpGet]
        public PaginatedResponse<IEnumerable<OutletResponse>> GetAll(int pageNumber, int pageSize, string? keyword)
        {
            var data = outletService.GetAll(pageNumber, pageSize, keyword);

            return new()
            {
                Response = data,
                CurrentPage = data.CurrentPage,
                TotalPages = data.TotalPages,
                TotalCount = data.TotalCount
            };
        }

        [HttpGet("{id}")]
        public async Task<OutletResponse> GetByIdAsync(string id)
        {
            return await outletService.GetByIdAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(OutletRequest outlet)
        {
            await outletService.CreateAsync(outlet);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, OutletRequest outlet)
        {
            await outletService.UpdateByIdAsync(id, outlet);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await outletService.DeleteByIdAsync(id);
            return Ok();
        }
    }
}
