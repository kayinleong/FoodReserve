namespace FoodReserve.SharedLibrary.Requests
{
    public class CustomerRequest
    {
        public string? UserId { get; set; }

        public string[]? ReservationIds { get; set; }

        public int? Status { get; set; }
    }
}
