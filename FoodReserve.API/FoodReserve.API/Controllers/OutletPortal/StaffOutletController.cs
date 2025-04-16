using FoodReserve.API.Services;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodReserve.API.Controllers.OutletPortal
{
    [ApiController]
    [Authorize(Roles = "Outlet")]
    [Route("api/outlet/staff")]
    public class StaffOutletController(StaffService staffService) : ControllerBase
    {
        [HttpGet("user/{id}")]
        public async Task<StaffResponse> GetByUserIdAsync(string id)
        {
            return await staffService.GetByUserIdAsync(id);
        }
    }
}
