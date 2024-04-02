using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http.Json;

namespace Front.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        public int userId {  get; set; }

        public string apiKey { get; set; }

        public AuthService(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("https://localhost:7239/api/Account/");
            _httpClient = httpClient;
        }

        //private int getUserIdFromToken(string token)
        //{
        //    var handler = new JwtSecurityTokenHandler();
        //    var jsonToken = handler.ReadToken(token);
        //    var tokenS = jsonToken as JwtSecurityToken;
        //}
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
            _httpClient.CancelPendingRequests();

            var response = await _httpClient.PostAsJsonAsync("register", loginData);
            if (response.IsSuccessStatusCode){
                var content = await response.Content.ReadAsStringAsync();
                if (content.Contains("token"))
                {
                    var token = content.Split()[1];
                    apiKey = token;
                    return "Успешная регистрация";
                }
                return "Регистрация не прошла\n"+content;
            }
            else return "Регистрация не прошла\n" + response.StatusCode;
        }

    }


}