namespace FoodReserve.SharedLibrary.Responses
{
    public class CustomerResponse : ApiResponse
    {
        public string? UserId { get; set; }

        public string[]? ReservationIds { get; set; }

        public int? Status { get; set; }
    }
}
