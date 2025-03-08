using FoodReserve.SharedLibrary.Constants;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using System.ComponentModel.DataAnnotations;

namespace FoodReserve.API.Models
{
    public class Reservation : BaseModel
    {
        [Required]
        public User? User { get; set; }

        [Required]
        public required Outlet Outlet { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string PhoneNumber { get; set; }

        [Required]
        public required string NumberOfGuest { get; set; }

        [Required]
        public required DateTime Date { get; set; }

        [Required]
        public required ReservationStatus Status { get; set; }

        public static explicit operator Reservation(ReservationRequest reservationRequest)
        {
            return new Reservation
            {
                User = null,
                Outlet = null,
                Name = reservationRequest.Name,
                PhoneNumber = reservationRequest.PhoneNumber,
                NumberOfGuest = reservationRequest.NumberOfGuest,
                Status = (ReservationStatus)reservationRequest.Status,
                Date = reservationRequest.Date,
            };
        }

        public static implicit operator ReservationResponse(Reservation reservation)
        {
            return new ReservationResponse
            {
                Id = reservation.Id,
                UserId = reservation.User?.Id,
                OutletId = reservation.Outlet.Id,
                Name = reservation.Name,
                PhoneNumber = reservation.PhoneNumber,
                NumberOfGuest = reservation.NumberOfGuest,
                Date = reservation.Date,
                Status = (int)reservation.Status,
                UpdatedAt = reservation.UpdatedAt,
                CreatedAt = reservation.CreatedAt,
            };
        }
    }
}
