using FoodReserve.API.Context;
using FoodReserve.API.Models;
using FoodReserve.SharedLibrary.Constants;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodReserve.API.Services
{
    public class UserService(DatabaseContext context, IPasswordHasher<User> passwordHasher)
    {
        public PagedList<UserResponse> GetAll(int pageNumber, int pageSize, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return PagedList<UserResponse>.ToPagedList(
                    context.Set<User>()
                        .Select(m => (UserResponse)m),
                    pageNumber, pageSize);
            }
            else
            {
                return PagedList<UserResponse>.ToPagedList(
                    context.Set<User>()
                        .Where(m => m.Username.Contains(keyword))
                        .Select(m => (UserResponse)m),
                    pageNumber, pageSize);
            }
        }

        public PagedList<UserResponse> GetAll(int pageNumber, int pageSize, UserRole role, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return PagedList<UserResponse>.ToPagedList(
                    context.Set<User>()
                        .Where(m => m.Role == role)
                        .Select(m => (UserResponse)m),
                    pageNumber, pageSize);
            }
            else
            {
                return PagedList<UserResponse>.ToPagedList(
                    context.Set<User>()
                        .Where(m => m.Role == role)
                        .Where(m => m.Username.Contains(keyword))
                        .Select(m => (UserResponse)m),
                    pageNumber, pageSize);
            }
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await context.Users
                .FirstOrDefaultAsync(m => m.Id == id) ?? throw new InvalidOperationException("User not found");
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await context.Users
                .FirstOrDefaultAsync(m => m.Username == username) ?? throw new InvalidOperationException("User not found");
        }

        public async Task CreateAsync(User user)
        {
            if (await context.Users.AnyAsync(m => m.Username == user.Username))
            {
                throw new InvalidOperationException("Username already exists");
            }

            if (await context.Users.AnyAsync(m => m.Email == user.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            user.PasswordHashed = passwordHasher.HashPassword(user, user.PasswordHashed);

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Create user failed");
            }
        }

        public async Task CreateAsync(UserCreateRequest userCreateRequest)
        {
            if (await context.Users.AnyAsync(m => m.Username == userCreateRequest.Username))
            {
                throw new InvalidOperationException("Username already exists");
            }

            if (await context.Users.AnyAsync(m => m.Email == userCreateRequest.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            User user = new()
            {
                Username = userCreateRequest.Username!,
                Email = userCreateRequest.Email!,
                PasswordHashed = "",
                IsSuspended = false
            };

            user.PasswordHashed = passwordHasher.HashPassword(user, userCreateRequest.Password!);

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Create user failed");
            }
        }

        public async Task UpdateByIdAsync(string id, User user)
        {
            var existingUser = await GetByIdAsync(id);

            if (user.Username != user.Username && await context.Users.AnyAsync(m => m.Username == user.Username))
            {
                throw new InvalidOperationException("Username already exists");
            }

            if (user.Email != user.Email && await context.Users.AnyAsync(m => m.Email == user.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            user.Username = user.Username;
            user.Email = user.Email;
            user.Role = user.Role;
            user.IsSuspended = user.IsSuspended;
            user.PasswordHashed = passwordHasher.HashPassword(user, user.PasswordHashed);

            try
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Update user failed");
            }
        }

        public async Task DeleteByIdAsync(string id)
        {
            var existingUser = await GetByIdAsync(id);

            try
            {
                context.Users.Remove(existingUser);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Delete user failed");
            }
        }
    }
}
