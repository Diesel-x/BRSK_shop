using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Components;

namespace Front.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        public int userId {  get; set; }
        public string apiKey { get; set; }

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            httpClient.BaseAddress = new Uri("https://localhost:7239/api/Account/");
            _httpClient = httpClient;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }

        public async Task<string> Login(string login, string password)
        {
            var loginData = new
            {
                login,
                password
            };
            _httpClient.CancelPendingRequests();

            var response = await _httpClient.PostAsJsonAsync("login", loginData);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (content.Contains("token"))
                {
                    var token = content.Split()[1];
                    apiKey = token;
                    await _localStorage.SetItemAsync("key", apiKey);
                    _navigationManager.NavigateTo("/", true);
                    return "Успешная авторизация";
                }
                return "Авторизация не прошла\n" + content;
            }
            else
            {
                // Обработка ошибки аутентификации
                return "Failed to login. Status code: " + response.StatusCode;
            }
        }

        public async Task<string> Register(string login, string password, string name){
            var loginData = new
            {
                login,
                password,
                name
            };
            //_httpClient.CancelPendingRequests();

            var response = await _httpClient.PostAsJsonAsync("register", loginData);
            if (response.IsSuccessStatusCode){
                var content = await response.Content.ReadAsStringAsync();
                if (content.Contains("token"))
                {
                    var token = content.Split()[1];
                    apiKey = token;
                    await _localStorage.SetItemAsync("key", apiKey);
                    _navigationManager.NavigateTo("/", true);
                    return "Успешная регистрация";

                }
                return "Регистрация не прошла\n"+content;
            }
            else return "Регистрация не прошла\n" + response.StatusCode;
        }

        public async Task Logout()
        {
            apiKey = null;
            await _localStorage.RemoveItemAsync("key");
        }
    }


}