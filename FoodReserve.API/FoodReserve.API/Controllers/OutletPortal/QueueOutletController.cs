using FoodReserve.API.Services;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodReserve.API.Controllers.OutletPortal
{
    [ApiController]
    [Authorize(Roles = "Outlet")]
    [Route("api/outlet/queue")]
    public class QueueOutletController(QueueService queueService) : ControllerBase
    {
        [HttpGet]
        public PaginatedResponse<IEnumerable<QueueResponse>> GetAll(int pageNumber, int pageSize, string? keyword)
        {
            var data = queueService.GetAll(pageNumber, pageSize, keyword);

            return new()
            {
                Response = data,
                CurrentPage = data.CurrentPage,
                TotalPages = data.TotalPages,
                TotalCount = data.TotalCount
            };
        }

        [HttpGet("{id}")]
        public async Task<QueueResponse> GetByIdAsync(string id)
        {
            return (QueueResponse)await queueService.GetByIdAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(QueueRequest data)
        {
            await queueService.CreateAsync(data);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, QueueRequest data)
        {
            await queueService.UpdateByIdAsync(id, data);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await queueService.DeleteByIdAsync(id);
            return Ok();
        }
    }
}
