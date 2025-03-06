using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using System.ComponentModel.DataAnnotations;

namespace FoodReserve.API.Models
{
    public class Outlet : BaseModel
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Location { get; set; }

        [Required]
        public required string OperatingHours { get; set; } = "[0900, 2000]";

        [Required]
        public required int Capacity { get; set; } = 100;

        public List<Staff> Staff { get; set; } = [];

        public static explicit operator OutletResponse(Outlet outlet)
        {
            return new OutletResponse
            {
                Id = outlet.Id,
                Name = outlet.Name,
                Location = outlet.Location,
                OperatingHours = outlet.OperatingHours,
                Capacity = outlet.Capacity,
                UpdatedAt = outlet.UpdatedAt,
                CreatedAt = outlet.CreatedAt,
            };
        }

        public static explicit operator Outlet(OutletRequest outletRequest)
        {
            return new Outlet
            {
                Name = outletRequest.Name,
                Location = outletRequest.Location,
                OperatingHours = outletRequest.OperatingHours,
                Capacity = outletRequest.Capacity,
            };
        }
    }
}
