namespace FoodReserve.SharedLibrary.Responses
{
    public class ReservationResponse : ApiResponse
    {
        public string? UserId { get; set; }

        public string? OutletId { get; set; }

        public string? Name { get; set; }

        public string? PhoneNumber { get; set; }

        public string? NumberOfGuest { get; set; }

        public int? Status { get; set; }

        public DateTime? Date { get; set; }
    }
}
