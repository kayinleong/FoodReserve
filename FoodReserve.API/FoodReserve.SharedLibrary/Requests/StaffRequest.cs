using System.ComponentModel.DataAnnotations;

namespace FoodReserve.SharedLibrary.Requests
{
    public class StaffRequest
    {
        [Required]
        public string? UserId { get; set; }

        [Required]
        public string[] OutletIds { get; set; } = [];

        [Required]
        public string? Permissions { get; set; }
    }
}
