using System.ComponentModel.DataAnnotations;

namespace FoodReserve.SharedLibrary.Responses
{
    public class OutletResponse : ApiResponse
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Location { get; set; }

        [Required]
        public required string OperatingHours { get; set; }

        [Required]
        public required int Capacity { get; set; }
    }
}
