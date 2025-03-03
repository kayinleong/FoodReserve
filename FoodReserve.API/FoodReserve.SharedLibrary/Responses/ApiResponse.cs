namespace FoodReserve.SharedLibrary.Responses
{
    public class ApiResponse
    {
        public required string Id { get; set; }

        public required DateTime UpdatedAt { get; set; }

        public required DateTime CreatedAt { get; set; }
    }
}
