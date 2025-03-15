using FoodReserve.API.Services;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FoodReserve.API.Controllers
{
    [ApiController]
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
    }
}
