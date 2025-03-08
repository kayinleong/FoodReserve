namespace FoodReserve.SharedLibrary.Responses
{
    public class StaffResponse : ApiResponse
    {
        public required string UserId { get; set; }

        public required string[] OutletIds { get; set; }

        public required string Permissions { get; set; }
    }
}
