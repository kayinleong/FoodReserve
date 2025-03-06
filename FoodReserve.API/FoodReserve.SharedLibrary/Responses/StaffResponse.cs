using System.ComponentModel.DataAnnotations;

namespace FoodReserve.SharedLibrary.Responses
{
    public class StaffResponse : ApiResponse
    {
        [Required]
        public required string UserId { get; set; }

        [Required]
        public required string[] OutletIds { get; set; }

        [Required]
        public required string Permissions { get; set; }
    }
}
