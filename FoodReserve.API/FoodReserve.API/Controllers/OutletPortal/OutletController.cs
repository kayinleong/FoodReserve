using FoodReserve.API.Services;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodReserve.API.Controllers.OutletPortal
{
    [ApiController]
    [Authorize(Roles = "Outlet")]
    [Route("api/outlet/outlet")]
    public class OutletController(OutletService outletService) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<OutletResponse> GetByIdAsync(string id)
        {
            return await outletService.GetByIdAsync(id);
        }
    }
}
