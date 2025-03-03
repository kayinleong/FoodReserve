namespace FoodReserve.SharedLibrary.Responses
{
    public class UserResponse : ApiResponse
    {
        public required string Username { get; set; }

        public required string Email { get; set; }

        public bool IsDeleted { get; set; } = false;

        public int Role { get; set; }

        public bool IsSuspended { get; set; } = false;
    }
}
