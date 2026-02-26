using AdminFelület.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace AdminFelület.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string? _accessToken;

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://cbnncff2-7114.euw.devtunnels.ms/api/")
            };
        }

        private void SetAuthHeader()
        {
            if (!string.IsNullOrEmpty(_accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            }
        }

        public async Task<LoginResponse?> LoginAsync(string username, string password)
        {
            try
            {
                var request = new LoginRequest
                {
                    Username = username,
                    Password = password
                };

                var response = await _httpClient.PostAsJsonAsync("Auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result != null)
                    {
                        _accessToken = result.AccessToken;
                        SetAuthHeader();
                    }
                    return result;
                }
            }
            catch { }
            return null;
        }

        public async Task<List<ReceptModel>> GetReceptekAsync()
        {
            try
            {
                var receptek = await _httpClient.GetFromJsonAsync<List<ReceptModel>>("Recept");
                return receptek ?? new List<ReceptModel>();
            }
            catch
            {
                return new List<ReceptModel>();
            }
        }

        public async Task<bool> DeleteReceptAsync(Guid id)
        {
            try
            {
                SetAuthHeader();
                var response = await _httpClient.DeleteAsync($"Recept/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<UserModel>> GetUsersAsync()
        {
            try
            {
                SetAuthHeader();
                var users = await _httpClient.GetFromJsonAsync<List<UserModel>>("Auth/admin/users");
                return users ?? new List<UserModel>();
            }
            catch
            {
                return new List<UserModel>();
            }
        }

        public async Task<bool> ChangeRoleAsync(Guid userId, string newRole)
        {
            try
            {
                SetAuthHeader();
                var response = await _httpClient.PutAsJsonAsync($"Auth/admin/users/{userId}/role", newRole);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            try
            {
                SetAuthHeader();
                var response = await _httpClient.DeleteAsync($"Auth/admin/users/{userId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<KommentModel>> GetKommentekAsync(Guid receptId)
        {
            try
            {
                var kommentek = await _httpClient.GetFromJsonAsync<List<KommentModel>>($"Komment/recept/{receptId}");
                return kommentek ?? new List<KommentModel>();
            }
            catch
            {
                return new List<KommentModel>();
            }
        }

        public async Task<bool> DeleteKommentAsync(Guid kommentId)
        {
            try
            {
                SetAuthHeader();
                var response = await _httpClient.DeleteAsync($"Komment/{kommentId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<StatisztikaModel?> GetStatisztikaAsync()
        {
            try
            {
                SetAuthHeader();
                var statisztika = await _httpClient.GetFromJsonAsync<StatisztikaModel>("Auth/admin/stats");
                return statisztika;
            }
            catch
            {
                return null;
            }
        }
    }
}