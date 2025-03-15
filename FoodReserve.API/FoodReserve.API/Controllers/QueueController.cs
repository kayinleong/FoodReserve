using FoodReserve.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodReserve.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueueController(QueueService queueService) : ControllerBase
    {
    }
}
