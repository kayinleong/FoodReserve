using FoodReserve.API.Context;
using FoodReserve.API.Models;
using FoodReserve.SharedLibrary.Constants;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using FoodReserve.API.Hubs;

namespace FoodReserve.API.Services
{
    public class QueueService(DatabaseContext context, IHubContext<QueueSignalRHub> hubContext)
    {
        public PagedList<QueueResponse> GetAll(int pageNumber, int pageSize, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return PagedList<QueueResponse>.ToPagedList(
                    context.Set<Queue>()
                        .Include(r => r.Outlet)
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(r => (QueueResponse)r),
                    pageNumber, pageSize);
            }
            else
            {
                return PagedList<QueueResponse>.ToPagedList(
                    context.Set<Queue>()
                        .Include(r => r.Outlet)
                        .OrderByDescending(m => m.CreatedAt)
                        .Where(r => r.Name.Contains(keyword) || r.PhoneNumber.Contains(keyword))
                        .Select(r => (QueueResponse)r),
                    pageNumber, pageSize);
            }
        }

        public PagedList<QueueResponse> GetAllByOutletId(int pageNumber, int pageSize, string outletId, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return PagedList<QueueResponse>.ToPagedList(
                    context.Set<Queue>()
                        .Include(r => r.Outlet)
                        .OrderByDescending(m => m.CreatedAt)
                        .Where(r => r.Outlet.Id == outletId)
                        .Select(r => (QueueResponse)r),
                    pageNumber, pageSize);
            }
            else
            {
                return PagedList<QueueResponse>.ToPagedList(
                    context.Set<Queue>()
                        .Include(r => r.Outlet)
                        .OrderByDescending(m => m.CreatedAt)
                        .Where(r => r.Outlet.Id == outletId)
                        .Where(r => r.Name.Contains(keyword) || r.PhoneNumber.Contains(keyword))
                        .Select(r => (QueueResponse)r),
                    pageNumber, pageSize);
            }
        }

        public async Task<Queue> GetByIdAsync(string id)
        {
            return await context.Queues
                .Include(r => r.Outlet)
                .FirstOrDefaultAsync(r => r.Id == id) ?? throw new InvalidOperationException("Queue not found");
        }

        private async Task<int> GetLatestQueueNumberByDateAsync(DateTime date, string outletId)
        {
            var latestQueue = await context.Queues
                .Where(q => q.Date.Date == date.Date && q.Outlet.Id == outletId)
                .OrderByDescending(q => q.QueueNumber)
                .FirstOrDefaultAsync();

            return latestQueue?.QueueNumber + 1 ?? 1;
        }

        public async Task CreateAsync(Queue queue)
        {
            try
            {
                await context.Queues.AddAsync(queue);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Create queue failed");
            }
        }

        public async Task CreateAsync(QueueRequest reservationRequest)
        {
            var outlet = await context.Outlets.FindAsync(reservationRequest.OutletId)
                ?? throw new InvalidOperationException("Outlet not found");

            int queueNumber = await GetLatestQueueNumberByDateAsync(reservationRequest.Date, reservationRequest.OutletId);

            Queue reservation = new()
            {
                Outlet = outlet,
                Name = reservationRequest.Name!,
                PhoneNumber = reservationRequest.PhoneNumber!,
                NumberOfGuest = reservationRequest.NumberOfGuest!,
                Date = reservationRequest.Date,
                QueueNumber = queueNumber,
                Status = (QueueStatus)reservationRequest.Status
            };

            try
            {
                await context.Queues.AddAsync(reservation);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Create queue failed");
            }
        }

        public async Task CreateAsync(QueueRequest reservationRequest, string id)
        {
            var outlet = await context.Outlets.FindAsync(reservationRequest.OutletId)
                ?? throw new InvalidOperationException("Outlet not found");

            int queueNumber = await GetLatestQueueNumberByDateAsync(reservationRequest.Date, reservationRequest.OutletId);

            Queue reservation = new()
            {
                Id = id,
                Outlet = outlet,
                Name = reservationRequest.Name!,
                PhoneNumber = reservationRequest.PhoneNumber!,
                NumberOfGuest = reservationRequest.NumberOfGuest!,
                Date = reservationRequest.Date,
                QueueNumber = queueNumber,
                Status = (QueueStatus)reservationRequest.Status
            };

            try
            {
                await context.Queues.AddAsync(reservation);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Create queue failed");
            }
        }

        public async Task UpdateByIdAsync(string id, QueueRequest reservationRequest)
        {
            var existingQueue = await GetByIdAsync(id);

            if (!string.IsNullOrEmpty(reservationRequest.OutletId))
            {
                var outlet = await context.Outlets.FindAsync(reservationRequest.OutletId)
                    ?? throw new InvalidOperationException("Outlet not found");
                existingQueue.Outlet = outlet;
            }

            if (existingQueue.Status != (QueueStatus)reservationRequest.Status)
            {
                await UpdateStatusAsync(existingQueue.Id, (QueueStatus)reservationRequest.Status);
            }

            existingQueue.Name = reservationRequest.Name ?? existingQueue.Name;
            existingQueue.PhoneNumber = reservationRequest.PhoneNumber ?? existingQueue.PhoneNumber;
            existingQueue.NumberOfGuest = reservationRequest.NumberOfGuest ?? existingQueue.NumberOfGuest;
            existingQueue.Date = reservationRequest.Date;

            try
            {
                context.Queues.Update(existingQueue);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Update reservation failed");
            }
        }

        public async Task UpdateStatusAsync(string id, QueueStatus status)
        {
            var queue = await GetByIdAsync(id);
            if (queue != null)
            {
                queue.Status = status;
                await context.SaveChangesAsync();
                
                // Use the injected hubContext instead of creating a new hub instance
                await hubContext.Clients.Group($"queue-{id}").SendAsync("QueueStatusUpdated", id, (int)status);
                
                if (status == QueueStatus.Approved)
                {
                    await hubContext.Clients.Group($"queue-{id}").SendAsync("QueueApproved", id);
                    
                    // Update positions for all remaining queues in this outlet
                    await UpdateQueuePositionsForOutlet(queue.Outlet.Id);
                }
            }
        }

        // Helper method to update positions for all queues in an outlet
        private async Task UpdateQueuePositionsForOutlet(string outletId)
        {
            // Get all pending queues for this outlet
            var queues = GetAllByOutletId(1, 100, outletId, string.Empty);
            var pendingQueues = queues.Where(q => q.Status == (int)QueueStatus.Pending)
                                     .OrderBy(q => q.QueueNumber)
                                     .ToList();
            
            // Update position for each queue
            for (int i = 0; i < pendingQueues.Count; i++)
            {
                await hubContext.Clients.Group($"queue-{pendingQueues[i].Id}")
                              .SendAsync("QueuePositionUpdated", pendingQueues[i].Id, i + 1);
            }
        }

        public async Task DeleteByIdAsync(string id)
        {
            var existingQueue = await GetByIdAsync(id);

            try
            {
                context.Queues.Remove(existingQueue);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Delete reservation failed");
            }
        }
    }
}
