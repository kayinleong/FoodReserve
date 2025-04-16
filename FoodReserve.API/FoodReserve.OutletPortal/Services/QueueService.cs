using FoodReserve.OutletPortal.Policies;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodReserve.OutletPortal.Services
{
    public class QueueService(ProtectedSessionStorage sessionStorage, IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("FoodReserve.API");
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            PropertyNameCaseInsensitive = true
        };

        public async Task InitializeAsync()
        {
            await SetAuthToken(sessionStorage);
        }

        public async Task<PaginatedResponse<IEnumerable<QueueResponse>>> GetAllAsync(int pageNumber, int pageSize, string outletId, string? keyword)
        {
            var response = await _httpClient.GetAsync($"api/outlet/queue?pageNumber={pageNumber}&pageSize={pageSize}&outletId={outletId}&keyword={keyword}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<PaginatedResponse<IEnumerable<QueueResponse>>>(_jsonOptions))!;
        }

        public async Task<QueueResponse> GetByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"api/outlet/queue/{id}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<QueueResponse>(_jsonOptions))!;
        }

        public async Task CreateAsync(QueueRequest queue)
        {
            var response = await _httpClient.PostAsJsonAsync("api/outlet/queue", queue, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(string id, QueueRequest queue)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/outlet/queue/{id}", queue, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/outlet/queue/{id}");
            response.EnsureSuccessStatusCode();
        }

        private async Task SetAuthToken(ProtectedSessionStorage sessionStorage)
        {
            var storedTokenResult = await sessionStorage.GetAsync<string>("authToken");
            var storedToken = storedTokenResult.Success ? storedTokenResult.Value : null;

            if (!string.IsNullOrEmpty(storedToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storedToken);
            }
        }
    }
}
