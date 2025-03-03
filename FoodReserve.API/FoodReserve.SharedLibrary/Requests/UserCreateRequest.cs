using System.ComponentModel.DataAnnotations;

namespace FoodReserve.SharedLibrary.Requests
{
    public class UserCreateRequest
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        [RegularExpression(@"^\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z$", ErrorMessage = "Email is invalid")]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; } = string.Empty;
    }
}
