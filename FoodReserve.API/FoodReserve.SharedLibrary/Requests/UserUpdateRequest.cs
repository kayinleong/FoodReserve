using System.ComponentModel.DataAnnotations;

namespace FoodReserve.SharedLibrary.Requests
{
    public class UserUpdateRequest
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        [RegularExpression(@"^\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z$", ErrorMessage = "Email is invalid")]
        public string? Email { get; set; }

        public string? Password { get; set; } = string.Empty;

        public bool IsSuspended { get; set; } = false;
    }
}
