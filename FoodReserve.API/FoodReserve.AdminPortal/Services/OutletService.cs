using FoodReserve.AdminPortal.Policies;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodReserve.AdminPortal.Services
{
    public class OutletService(ProtectedSessionStorage sessionStorage, IHttpClientFactory httpClientFactory)
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

        public async Task<PaginatedResponse<IEnumerable<OutletResponse>>> GetAllAsync(int pageNumber, int pageSize, string? keyword)
        {
            var response = await _httpClient.GetAsync($"api/outlet?pageNumber={pageNumber}&pageSize={pageSize}&keyword={keyword}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<PaginatedResponse<IEnumerable<OutletResponse>>>(_jsonOptions))!;
        }

        public async Task<OutletResponse> GetByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"api/outlet/{id}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<OutletResponse>(_jsonOptions))!;
        }

        public async Task CreateAsync(OutletRequest outlet)
        {
            var response = await _httpClient.PostAsJsonAsync("api/outlet", outlet, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(string id, OutletRequest outlet)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/outlet/{id}", outlet, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/outlet/{id}");
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
