using FoodReserve.AdminPortal.Policies;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodReserve.AdminPortal.Services
{
    public class OutletUserService(ProtectedSessionStorage sessionStorage, IHttpClientFactory httpClientFactory)
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

        public async Task<PaginatedResponse<IEnumerable<UserResponse>>> GetAllAsync(int pageNumber, int pageSize, string? keyword)
        {
            var response = await _httpClient.GetAsync($"api/outletuser?pageNumber={pageNumber}&pageSize={pageSize}&keyword={keyword}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<PaginatedResponse<IEnumerable<UserResponse>>>(_jsonOptions))!;
        }

        public async Task<UserResponse> GetByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"api/outletuser/{id}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<UserResponse>(_jsonOptions))!;
        }

        public async Task<UserResponse> GetByUsernameAsync(string username)
        {
            var response = await _httpClient.GetAsync($"api/outletuser/username/{username}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<UserResponse>(_jsonOptions))!;
        }

        public async Task CreateAsync(UserCreateRequest user)
        {
            var response = await _httpClient.PostAsJsonAsync("api/outletuser", user, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(string id, UserUpdateRequest user)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/outletuser/{id}", user, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/outletuser/{id}");
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
