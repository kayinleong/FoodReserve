using FoodReserve.API.Models;
using FoodReserve.API.Services;
using FoodReserve.SharedLibrary.Constants;
using FoodReserve.SharedLibrary.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FoodReserve.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController(ReservationService reservationService, WhatsAppService whatsAppService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateAsync(ReservationRequest data)
        {
            await reservationService.CreateAsync(data);

            await whatsAppService.SendMessage(new
            {
                MessagingProduct = "whatsapp",
                To = data.PhoneNumber,
                Type = "template",
                Template = new
                {
                    Name = "reservation_confirm",
                    Language = new
                    {
                        Code = "en"
                    },
                    Components = new List<object>()
                    {
                        new {
                            Type = "body",
                            Parameters = new {
                                Type = "text",
                                Text = data.Name
                            }
                        }
                    }
                }
            });

            return Ok();
        }
    }
}
