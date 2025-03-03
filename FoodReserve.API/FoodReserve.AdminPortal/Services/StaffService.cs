using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;

namespace FoodReserve.AdminPortal.Services
{
    public class StaffService
    {
        private readonly HttpClient _httpClient;

        public StaffService(ProtectedSessionStorage sessionStorage, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FoodReserve.API");
            SetAuthToken(sessionStorage).Wait();
        }

        public async Task<PaginatedResponse<IEnumerable<UserResponse>>> GetAllAsync(int pageNumber, int pageSize, string? keyword)
        {
            var response = await _httpClient.GetAsync($"api/staff?pageNumber={pageNumber}&pageSize={pageSize}&keyword={keyword}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<PaginatedResponse<IEnumerable<UserResponse>>>())!;
        }

        public async Task<UserResponse> GetByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"api/staff/{id}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<UserResponse>())!;
        }

        public async Task<UserResponse> GetByUsernameAsync(string username)
        {
            var response = await _httpClient.GetAsync($"api/staff/username/{username}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<UserResponse>())!;
        }

        public async Task CreateAsync(UserCreateRequest user)
        {
            var response = await _httpClient.PostAsJsonAsync("api/staff", user);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(string id, UserUpdateRequest user)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/staff/{id}", user);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/staff/{id}");
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
