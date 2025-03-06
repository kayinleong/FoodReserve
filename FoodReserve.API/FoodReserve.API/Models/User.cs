using FoodReserve.SharedLibrary.Constants;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using System.ComponentModel.DataAnnotations;

namespace FoodReserve.API.Models
{
    public class User : BaseModel
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        [RegularExpression(@"^\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z$", ErrorMessage = "Email is invalid")]
        public required string Email { get; set; }

        [Required]
        public string PasswordHashed { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; } = UserRole.GUEST;

        [Required]
        public bool IsSuspended { get; set; } = false;

        public static implicit operator UserResponse(User user)
        {
            return new()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = (int)user.Role,
                IsSuspended = user.IsSuspended,
                IsDeleted = user.IsDeleted,
                UpdatedAt = user.UpdatedAt,
                CreatedAt = user.CreatedAt
            };
        }

        public static explicit operator User(UserCreateRequest userCreateRequest)
        {
            return new()
            {
                Username = userCreateRequest.Username!,
                Email = userCreateRequest.Email!,
                PasswordHashed = userCreateRequest.Password!,
                Role = UserRole.GUEST
            };
        }

        public static explicit operator User(UserUpdateRequest userUpdateRequest)
        {
            return new()
            {
                Username = userUpdateRequest.Username!,
                Email = userUpdateRequest.Email!,
                PasswordHashed = userUpdateRequest.Password!,
                Role = UserRole.GUEST,
                IsSuspended = userUpdateRequest.IsSuspended 
            };
        }
    }
}
