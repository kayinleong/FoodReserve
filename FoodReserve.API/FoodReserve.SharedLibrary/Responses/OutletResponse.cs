namespace FoodReserve.SharedLibrary.Responses
{
    public class OutletResponse : ApiResponse
    {
        public required string Name { get; set; }

        public required string Location { get; set; }

        public required string OperatingHours { get; set; }

        public required int Capacity { get; set; }
    }
}
