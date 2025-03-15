using FoodReserve.API.Context;
using FoodReserve.API.Models;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;

namespace FoodReserve.API.Services
{
    public class OutletService(DatabaseContext context)
    {
        public PagedList<OutletResponse> GetAll(int pageNumber, int pageSize, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return PagedList<OutletResponse>.ToPagedList(
                    context.Set<Outlet>()
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => (OutletResponse)m),
                    pageNumber, pageSize);
            }
            else
            {
                return PagedList<OutletResponse>.ToPagedList(
                    context.Set<Outlet>()
                        .Where(m => m.Name.Contains(keyword))
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => (OutletResponse)m),
                    pageNumber, pageSize);
            }
        }

        public async Task<OutletResponse> GetByIdAsync(string id)
        {
            var outlet = await context.Outlets
                .Include(o => o.Staff)
                .FirstOrDefaultAsync(o => o.Id == id)
                ?? throw new KeyNotFoundException($"Outlet with ID {id} not found");

            return (OutletResponse)outlet;
        }

        public async Task<IEnumerable<OutletResponse>> GetByStaffIdAsync(string staffId)
        {
            var outlets = await context.Outlets
                .Include(o => o.Staff)
                .Where(o => o.Staff.Any(s => s.Id == staffId))
                .Select(o => (OutletResponse)(object)o)
                .ToListAsync();

            return outlets;
        }

        public async Task<OutletResponse> CreateAsync(OutletRequest request)
        {
            var outlet = (Outlet)request;
            
            await context.Outlets.AddAsync(outlet);
            await context.SaveChangesAsync();

            return await GetByIdAsync(outlet.Id);
        }

        public async Task<OutletResponse> UpdateByIdAsync(string id, OutletRequest request)
        {
            var outlet = await context.Outlets
                .Include(o => o.Staff)
                .FirstOrDefaultAsync(o => o.Id == id)
                ?? throw new KeyNotFoundException($"Outlet with ID {id} not found");

            outlet.Name = request.Name;
            outlet.Location = request.Location;
            outlet.OperatingHours = request.OperatingHours;
            outlet.Capacity = request.Capacity;
            outlet.UpdatedAt = DateTime.UtcNow;

            context.Outlets.Update(outlet);
            await context.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task DeleteByIdAsync(string id)
        {
            var outlet = await context.Outlets.FindAsync(id)
                ?? throw new KeyNotFoundException($"Outlet with ID {id} not found");

            context.Outlets.Remove(outlet);
            await context.SaveChangesAsync();
        }
    }
}
