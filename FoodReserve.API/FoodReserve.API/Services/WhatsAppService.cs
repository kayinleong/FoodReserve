using System.Net.Http.Headers;

namespace FoodReserve.API.Services
{
    public class WhatsAppService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("FoodReserve.API");

        public async Task SendMessage(object whatsAppRequest)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration["Meta.WhatsApp.Token"]);

            var response = await _httpClient.PostAsJsonAsync(
                $"/{configuration["Meta.WhatsApp.Version"]}/{configuration["Meta.WhatsApp.From"]}/messages", whatsAppRequest);

            response.EnsureSuccessStatusCode();
        }
    }
}
