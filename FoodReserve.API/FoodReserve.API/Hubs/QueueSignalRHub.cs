using FoodReserve.API.Services;
using FoodReserve.SharedLibrary.Constants;
using FoodReserve.SharedLibrary.Requests;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace FoodReserve.API.Hubs
{
    public class QueueSignalRHub(QueueService queueService) : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _connectionMap = new();

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Remove client connection from tracking
            if (_connectionMap.TryRemove(Context.ConnectionId, out string? queueId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"queue-{queueId}");
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        public async Task CreateQueue(QueueRequest queueRequest)
        {
            var id = Guid.NewGuid().ToString();
            await queueService.CreateAsync(queueRequest, id);
            
            // Get latest queue to retrieve its ID and position information
            var queues = queueService.GetAllByOutletId(1, 50, queueRequest.OutletId, string.Empty);
            
            // Find the newly created queue
            var newQueue = queues.FirstOrDefault(m => m.Id == id);
            
            if (newQueue != null)
            {
                // Add connection to the queue's group
                _connectionMap[Context.ConnectionId] = newQueue.Id;
                await Groups.AddToGroupAsync(Context.ConnectionId, $"queue-{newQueue.Id}");
                
                // Calculate position (number of queues with lower queue number that are still pending)
                int position = queues.Count(q => 
                    q.QueueNumber < newQueue.QueueNumber && 
                    q.Status == (int)QueueStatus.Pending);
                
                // Notify client about their position
                await Clients.Caller.SendAsync("QueueCreated", newQueue.Id, newQueue.Name, newQueue.NumberOfGuest, newQueue.QueueNumber, position + 1);
            }
        }

        public async Task JoinQueue(string queueId)
        {
            // Check if queue exists
            var queue = await queueService.GetByIdAsync(queueId);
            if (queue != null)
            {
                // Track this connection with the queue
                _connectionMap[Context.ConnectionId] = queueId;
                await Groups.AddToGroupAsync(Context.ConnectionId, $"queue-{queueId}");
                
                // Calculate position
                var queues = queueService.GetAllByOutletId(1, 50, queue.Outlet.Id, string.Empty);
                int position = queues.Count(q => 
                    q.QueueNumber < queue.QueueNumber && 
                    q.Status == (int)QueueStatus.Pending);
                
                await Clients.Caller.SendAsync("QueueJoined", queueId, queue.QueueNumber, position + 1);
            }
        }
        
        public async Task LeaveQueue(string queueId)
        {
            if (_connectionMap.TryRemove(Context.ConnectionId, out _))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"queue-{queueId}");
            }
        }
    }
}
