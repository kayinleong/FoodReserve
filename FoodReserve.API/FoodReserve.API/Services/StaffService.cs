using FoodReserve.API.Context;
using FoodReserve.API.Models;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;

namespace FoodReserve.API.Services
{
    public class StaffService(DatabaseContext context)
    {
        public PagedList<StaffResponse> GetAll(int pageNumber, int pageSize, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return PagedList<StaffResponse>.ToPagedList(
                    context.Set<Staff>()
                        .Include(m => m.User)
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => (StaffResponse)m),
                    pageNumber, pageSize);
            }
            else
            {
                return PagedList<StaffResponse>.ToPagedList(
                    context.Set<Staff>()
                        .Include(m => m.User)
                        .OrderByDescending(m => m.CreatedAt)
                        .Where(m => m.User.Username.Contains(keyword))
                        .Select(m => (StaffResponse)m),
                    pageNumber, pageSize);
            }
        }

        public async Task<StaffResponse> GetByIdAsync(string id)
        {
            var staff = await context.Staff
                .Include(s => s.User)
                .Include(s => s.Outlets)
                .FirstOrDefaultAsync(s => s.Id == id)
                ?? throw new KeyNotFoundException($"Staff with ID {id} not found");

            return staff;
        }

        public async Task<StaffResponse> GetByUserIdAsync(string userId)
        {
            var staff = await context.Staff
                .Include(s => s.User)
                .Include(s => s.Outlets)
                .FirstOrDefaultAsync(s => s.User.Id == userId)
                ?? throw new KeyNotFoundException($"Staff with User ID {userId} not found");

            return staff;
        }

        public async Task<IEnumerable<StaffResponse>> GetByOutletIdAsync(string outletId)
        {
            var staffList = await context.Staff
                .Include(s => s.User)
                .Include(s => s.Outlets)
                .Where(s => s.Outlets.Any(o => o.Id == outletId))
                .Select(s => (StaffResponse)s)
                .ToListAsync();

            return staffList;
        }

        public async Task<StaffResponse> CreateAsync(StaffRequest request, string userId)
        {
            var user = await context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException($"User with ID {userId} not found");

            List<Outlet> outlets = [];
            if (request.OutletIds.Length != 0)
            {
                outlets = [.. context.Outlets.Where(o => request.OutletIds.Contains(o.Id))];
            }

            var staff = (Staff)request;
            staff.User = user;
            staff.Outlets = [.. outlets];

            await context.Staff.AddAsync(staff);
            await context.SaveChangesAsync();

            return await GetByIdAsync(staff.Id);
        }

        public async Task<StaffResponse> UpdateByIdAsync(string id, StaffRequest request)
        {
            var staff = await context.Staff
                .Include(s => s.User)
                .Include(s => s.Outlets)
                .FirstOrDefaultAsync(s => s.Id == id)
                ?? throw new KeyNotFoundException($"Staff with ID {id} not found");

            if (request.OutletIds.Length != 0)
            {
                var outlets = await context.Outlets
                    .Where(o => request.OutletIds.Contains(o.Id))
                    .ToListAsync();
                
                staff.Outlets = outlets;
            }

            staff.Permissions = request.Permissions;
            staff.UpdatedAt = DateTime.UtcNow;

            context.Staff.Update(staff);
            await context.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task DeleteByIdAsync(string id)
        {
            var staff = await context.Staff.FindAsync(id)
                ?? throw new KeyNotFoundException($"Staff with ID {id} not found");

            context.Staff.Remove(staff);
            await context.SaveChangesAsync();
        }
    }
}
