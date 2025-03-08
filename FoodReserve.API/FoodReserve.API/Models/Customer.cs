using FoodReserve.SharedLibrary.Constants;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using System.ComponentModel.DataAnnotations;

namespace FoodReserve.API.Models
{
    public class Customer : BaseModel
    {
        [Required]
        public required User User { get; set; }

        [Required]
        public required List<Reservation> Reservations { get; set; }

        [Required]
        public required CustomerStatus Status { get; set; } = CustomerStatus.Active;

        public static implicit operator Customer(CustomerRequest customerRequest)
        {
            return new()
            {
                User = null,
                Reservations = null,
                Status = (CustomerStatus)customerRequest.Status
            };
        }

        public static explicit operator CustomerResponse(Customer customer)
        {
            return new()
            {
                Id = customer.Id,
                UserId = customer.User.Id,
                ReservationIds = [.. customer.Reservations.Select(m => m.Id)],
                Status = (int)customer.Status,
                UpdatedAt = customer.UpdatedAt,
                CreatedAt = customer.CreatedAt
            };
        }
    }
}
