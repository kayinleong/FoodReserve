namespace FoodReserve.SharedLibrary.Requests
{
    public class QueueRequest
    {
        public required string OutletId { get; set; }

        public required string Name { get; set; }

        public required string PhoneNumber { get; set; }

        public required string NumberOfGuest { get; set; }

        public required int QueueNumber { get; set; }

        public required int Status { get; set; }

        public required DateTime Date { get; set; }
    }
}
