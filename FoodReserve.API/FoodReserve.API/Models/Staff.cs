using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using System.ComponentModel.DataAnnotations;

namespace FoodReserve.API.Models
{
    public class Staff : BaseModel
    {
        [Required]
        public required User User { get; set; }

        [Required]
        public List<Outlet> Outlets { get; set; } = [];

        [Required]
        public required string Permissions { get; set; }

        public static implicit operator StaffResponse(Staff staff)
        {
            return new StaffResponse
            {
                Id = staff.Id,
                UserId = staff.User.Id,
                OutletIds = [.. staff.Outlets.Select(outlet => outlet.Id)],
                Permissions = staff.Permissions,
                UpdatedAt = staff.UpdatedAt,
                CreatedAt = staff.CreatedAt,
            };
        }

        public static explicit operator Staff(StaffRequest staffRequest)
        {
            return new Staff
            {
                User = null,
                Outlets = null,
                Permissions = staffRequest.Permissions,
            };
        }
    }
}
