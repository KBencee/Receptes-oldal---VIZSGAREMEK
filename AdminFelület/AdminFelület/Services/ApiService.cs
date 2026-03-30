using AdminFelület.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                BaseAddress = new Uri("https://localhost:7114/api/")
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
                    var tokenResult = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
                    if (tokenResult == null || string.IsNullOrEmpty(tokenResult.AccessToken))
                    {
                        Console.WriteLine("Login: token response was null or missing access token.");
                        return null;
                    }

                    _accessToken = tokenResult.AccessToken;
                    SetAuthHeader();

                    try
                    {
                        var userDto = await _httpClient.GetFromJsonAsync<UserResponseDto>("Auth/me");
                        if (userDto == null)
                        {
                            Console.WriteLine("Login: could not retrieve user info from /Auth/me.");
                            return null;
                        }

                        var loginResponse = new LoginResponse
                        {
                            AccessToken = tokenResult.AccessToken,
                            RefreshToken = tokenResult.RefreshToken,
                            User = new UserModel
                            {
                                Id = userDto.Id,
                                Username = userDto.Username,
                                Role = userDto.Role,
                                ProfilKepUrl = userDto.ProfileImageUrl
                            }
                        };

                        Console.WriteLine($"AccessToken: {loginResponse.AccessToken}");
                        Console.WriteLine($"User.Username: {loginResponse.User?.Username}");
                        Console.WriteLine($"User.Role: {loginResponse.User?.Role}");

                        return loginResponse;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to get /Auth/me: {ex.Message}");
                        return null;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Login hiba: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Exception: {ex.Message}");
            }
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

        public async Task<bool> UpdateReceptAsync(ReceptModel recept)
        {
            try
            {
                SetAuthHeader();
                var dto = new
                {
                    recept.Nev,
                    recept.Leiras,
                    recept.Hozzavalok,
                    recept.ElkeszitesiIdo,
                    recept.NehezsegiSzint
                };
                var response = await _httpClient.PutAsJsonAsync($"Auth/admin/receptek/{recept.Id}", dto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
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

        public async Task<bool> UpdateUserAsync(UserModel user)
        {
            try
            {
                SetAuthHeader();
                var dto = new { Username = user.Username, Role = user.Role };
                var response = await _httpClient.PutAsJsonAsync($"Auth/admin/users/{user.Id}", dto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
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

        public async Task<List<CimkeModel>> GetCimkekAsync()
        {
            try
            {
                SetAuthHeader();
                return await _httpClient.GetFromJsonAsync<List<CimkeModel>>("Auth/admin/cimkek")
                    ?? new List<CimkeModel>();
            }
            catch { return new List<CimkeModel>(); }
        }

        public async Task<bool> UpdateCimkeAsync(CimkeModel cimke)
        {
            try
            {
                SetAuthHeader();
                var dto = new { CimkeNev = cimke.CimkeNev };
                var response = await _httpClient.PutAsJsonAsync($"Auth/admin/cimkek/{cimke.Id}", dto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> DeleteCimkeAsync(int id)
        {
            try
            {
                SetAuthHeader();
                var response = await _httpClient.DeleteAsync($"Auth/admin/cimkek/{id}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<List<KommentModel>> GetAllKommentekAsync()
        {
            try
            {
                SetAuthHeader();
                var result = await _httpClient.GetFromJsonAsync<List<KommentModel>>("Auth/admin/kommentek") ?? new List<KommentModel>();

                Debug.WriteLine($"GetAllKommentekAsync: {result.Count} elem kapott");
                if (result.Count > 0)
                    Debug.WriteLine($"Első komment: Username='{result[0].Username}', ReceptNev='{result[0].ReceptNev}'");

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAllKommentekAsync hiba: {ex.Message}");
                return new List<KommentModel>();
            }
        }
        public async Task<bool> UpdateKommentAsync(KommentModel komment)
        {
            try
            {
                SetAuthHeader();
                var dto = new { Szoveg = komment.Szoveg };
                var response = await _httpClient.PutAsJsonAsync($"Auth/admin/kommentek/{komment.Id}", dto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> DeleteKommentAsync(Guid id)
        {
            try
            {
                SetAuthHeader();
                var response = await _httpClient.DeleteAsync($"Komment/{id}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<List<MentettReceptModel>> GetMentettReceptekAsync()
        {
            try
            {
                SetAuthHeader();
                var result = await _httpClient.GetFromJsonAsync<List<MentettReceptModel>>("Auth/admin/mentett-receptek") ?? new List<MentettReceptModel>();

                Debug.WriteLine($"GetMentettReceptekAsync: {result.Count} elem kapott");
                if (result.Count > 0)
                    Debug.WriteLine($"Első elem: Username='{result[0].Username}', ReceptNev='{result[0].ReceptNev}'");

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetMentettReceptekAsync hiba: {ex.Message}");
                return new List<MentettReceptModel>();
            }
        }

        public async Task<bool> DeleteMentettReceptAsync(Guid id)
        {
            try
            {
                SetAuthHeader();
                var response = await _httpClient.DeleteAsync($"Auth/admin/mentett-receptek/{id}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<List<LikeModel>> GetLikesAsync()
        {
            try
            {
                SetAuthHeader();
                var result = await _httpClient.GetFromJsonAsync<List<LikeModel>>("Auth/admin/likes") ?? new List<LikeModel>();

                Debug.WriteLine($"GetLikesAsync: {result.Count} elem kapott");
                if (result.Count > 0)
                    Debug.WriteLine($"Első like: Username='{result[0].Username}', ReceptNev='{result[0].ReceptNev}'");

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetLikesAsync hiba: {ex.Message}");
                return new List<LikeModel>();
            }
        }

        public async Task<List<ReceptCimkeModel>> GetReceptCimkekAsync()
        {
            try
            {
                SetAuthHeader();
                var result = await _httpClient.GetFromJsonAsync<List<ReceptCimkeModel>>("Auth/admin/recept-cimkek") ?? new List<ReceptCimkeModel>();

                Debug.WriteLine($"GetReceptCimkekAsync: {result.Count} elem kapott");
                if (result.Count > 0)
                    Debug.WriteLine($"Első elem: ReceptNev='{result[0].ReceptNev}', CimkeNev='{result[0].CimkeNev}'");

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetReceptCimkekAsync hiba: {ex.Message}");
                return new List<ReceptCimkeModel>();
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