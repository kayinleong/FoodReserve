using FoodReserve.AdminPortal.Policies;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodReserve.AdminPortal.Services
{
    public class OutletStaffService(ProtectedSessionStorage sessionStorage, IHttpClientFactory httpClientFactory)
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

        public async Task<PaginatedResponse<IEnumerable<StaffResponse>>> GetAllAsync(int pageNumber, int pageSize, string? keyword)
        {
            var response = await _httpClient.GetAsync($"api/admin/outletstaff?pageNumber={pageNumber}&pageSize={pageSize}&keyword={keyword}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<PaginatedResponse<IEnumerable<StaffResponse>>>(_jsonOptions))!;
        }

        public async Task<StaffResponse> GetByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"api/admin/outletstaff/{id}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<StaffResponse>(_jsonOptions))!;
        }

        public async Task<StaffResponse> GetByUserIdAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"api/admin/outletstaff/user/{userId}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<StaffResponse>(_jsonOptions))!;
        }

        public async Task<StaffResponse?> TryGetByUserIdAsync(string userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/admin/outletstaff/user/{userId}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<StaffResponse>(_jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        public async Task CreateAsync(StaffRequest staff)
        {
            var response = await _httpClient.PostAsJsonAsync("api/admin/outletstaff", staff, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(string id, StaffRequest staff)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/admin/outletstaff/{id}", staff, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/admin/outletstaff/{id}");
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
