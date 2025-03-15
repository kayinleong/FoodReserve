namespace FoodReserve.SharedLibrary.Requests
{
    public class ReservationRequest
    {
        public string? UserId { get; set; }

        public required string OutletId { get; set; }

        public required string Name { get; set; }

        public required string PhoneNumber { get; set; }

        public required int NumberOfGuest { get; set; }

        public required int Status { get; set; }

        public required DateTime Date { get; set; }
    }
}
