﻿using FoodReserve.OutletPortal.Policies;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodReserve.OutletPortal.Services
{
    public class StaffService(ProtectedSessionStorage sessionStorage, IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("FoodReserve.API");
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            PropertyNameCaseInsensitive = true
        };

        public async Task<StaffResponse> GetByUserIdAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"api/outlet/staff/user/{userId}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<StaffResponse>(_jsonOptions))!;
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

        public async Task InitializeAsync()
        {
            await SetAuthToken(sessionStorage);
        }
    }
}
