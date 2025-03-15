using FoodReserve.OutletPortal.Policies;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodReserve.OutletPortal.Services
{
    public class ReservationService(ProtectedSessionStorage sessionStorage, IHttpClientFactory httpClientFactory)
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

        public async Task<PaginatedResponse<IEnumerable<ReservationResponse>>> GetAllAsync(int pageNumber, int pageSize, string? keyword)
        {
            var response = await _httpClient.GetAsync($"api/outlet/reservation?pageNumber={pageNumber}&pageSize={pageSize}&keyword={keyword}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<PaginatedResponse<IEnumerable<ReservationResponse>>>(_jsonOptions))!;
        }

        public async Task<PaginatedResponse<IEnumerable<ReservationResponse>>> GetAllByUserIdAsync(int pageNumber, int pageSize, string? userId)
        {
            var response = await _httpClient.GetAsync($"api/outlet/reservation/user?pageNumber={pageNumber}&pageSize={pageSize}&userId={userId}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<PaginatedResponse<IEnumerable<ReservationResponse>>>(_jsonOptions))!;
        }

        public async Task<ReservationResponse> GetByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"api/outlet/reservation/{id}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<ReservationResponse>(_jsonOptions))!;
        }

        public async Task CreateAsync(ReservationRequest reservation)
        {
            var response = await _httpClient.PostAsJsonAsync("api/outlet/reservation", reservation, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(string id, ReservationRequest reservation)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/outlet/reservation/{id}", reservation, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/outlet/reservation/{id}");
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
