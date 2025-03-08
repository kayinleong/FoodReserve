using FoodReserve.SharedLibrary.Responses;

namespace FoodReserve.SharedLibrary.Requests
{
    public class ReservationRequest : ApiResponse
    {
        public string? UserId { get; set; }

        public required string OutletId { get; set; }

        public required string Name { get; set; }

        public required string PhoneNumber { get; set; }

        public required string NumberOfGuest { get; set; }

        public required int Status { get; set; }

        public required DateTime Date { get; set; }
    }
}
