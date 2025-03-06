using System.ComponentModel.DataAnnotations;

namespace FoodReserve.SharedLibrary.Requests
{
    public class OutletRequest
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Location { get; set; }

        [Required]
        public string? OperatingHours { get; set; }

        [Required]
        public int Capacity { get; set; }
    }
}
