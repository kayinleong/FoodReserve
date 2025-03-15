using FoodReserve.SharedLibrary.Constants;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using System.ComponentModel.DataAnnotations;

namespace FoodReserve.API.Models
{
    public class Queue : BaseModel
    {
        [Required]
        public required Outlet Outlet { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string PhoneNumber { get; set; }

        [Required]
        public required string NumberOfGuest { get; set; }

        [Required]
        public required int QueueNumber { get; set; }

        [Required]
        public required DateTime Date { get; set; }

        [Required]
        public required QueueStatus Status { get; set; }

        public static explicit operator Queue(QueueRequest queueRequest)
        {
            return new Queue
            {
                Outlet = null,
                Name = queueRequest.Name,
                PhoneNumber = queueRequest.PhoneNumber,
                NumberOfGuest = queueRequest.NumberOfGuest,
                QueueNumber = queueRequest.QueueNumber, 
                Status = (QueueStatus)queueRequest.Status,
                Date = queueRequest.Date,
            };
        }

        public static implicit operator QueueResponse(Queue queue)
        {
            return new QueueResponse
            {
                Id = queue.Id,
                OutletId = queue.Outlet.Id,
                Name = queue.Name,
                PhoneNumber = queue.PhoneNumber,
                NumberOfGuest = queue.NumberOfGuest,
                QueueNumber = queue.QueueNumber,
                Date = queue.Date,
                Status = (int)queue.Status,
                UpdatedAt = queue.UpdatedAt,
                CreatedAt = queue.CreatedAt,
            };
        }
    }
}
