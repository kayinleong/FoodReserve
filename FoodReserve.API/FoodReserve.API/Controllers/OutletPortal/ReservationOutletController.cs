using FoodReserve.API.Services;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodReserve.API.Controllers.OutletPortal
{
    [ApiController]
    [Authorize(Roles = "Outlet")]
    [Route("api/outlet/reservation")]
    public class ReservationOutletController(ReservationService reservationService) : ControllerBase
    {
        [HttpGet]
        public PaginatedResponse<IEnumerable<ReservationResponse>> GetAllByOutletId(int pageNumber, int pageSize, string outletId, string? keyword)
        {
            var data = reservationService.GetAllByOutletId(pageNumber, pageSize, outletId, keyword);

            return new()
            {
                Response = data,
                CurrentPage = data.CurrentPage,
                TotalPages = data.TotalPages,
                TotalCount = data.TotalCount
            };
        }

        [HttpGet("user")]
        public PaginatedResponse<IEnumerable<ReservationResponse>> GetAllByUserId(int pageNumber, int pageSize, string userId)
        {
            var data = reservationService.GetAllByUserId(pageNumber, pageSize, userId);

            return new()
            {
                Response = data,
                CurrentPage = data.CurrentPage,
                TotalPages = data.TotalPages,
                TotalCount = data.TotalCount
            };
        }

        [HttpGet("{id}")]
        public async Task<ReservationResponse> GetByIdAsync(string id)
        {
            return (ReservationResponse)await reservationService.GetByIdAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(ReservationRequest data)
        {
            await reservationService.CreateAsync(data);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, ReservationRequest data)
        {
            await reservationService.UpdateByIdAsync(id, data);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await reservationService.DeleteByIdAsync(id);
            return Ok();
        }
    }
}
