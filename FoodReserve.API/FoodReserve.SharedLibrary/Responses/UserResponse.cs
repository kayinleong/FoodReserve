namespace FoodReserve.SharedLibrary.Responses
{
    public class UserResponse : ApiResponse
    {
        public string? Username { get; set; }

        public string? Email { get; set; }

        public bool IsDeleted { get; set; } = false;

        public int Role { get; set; }

        public bool IsSuspended { get; set; } = false;
    }
}
