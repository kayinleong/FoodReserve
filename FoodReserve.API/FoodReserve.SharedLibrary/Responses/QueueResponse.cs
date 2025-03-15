namespace FoodReserve.SharedLibrary.Responses
{
    public class QueueResponse : ApiResponse
    {
        public string? OutletId { get; set; }

        public string? Name { get; set; }

        public string? PhoneNumber { get; set; }

        public string? NumberOfGuest { get; set; }

        public int? QueueNumber { get; set; }

        public int? Status { get; set; }

        public DateTime? Date { get; set; }
    }
}
