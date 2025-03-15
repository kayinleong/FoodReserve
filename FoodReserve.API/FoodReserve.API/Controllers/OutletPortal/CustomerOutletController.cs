using FoodReserve.API.Services;
using FoodReserve.SharedLibrary.Constants;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodReserve.API.Controllers.OutletPortal
{
    [ApiController]
    [Authorize(Roles = "Outlet")]
    [Route("api/outlet/customer")]
    public class CustomerOutletController(UserService userService, CustomerService customerService) : ControllerBase
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
    }
}
