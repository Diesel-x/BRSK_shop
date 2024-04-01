using Front.Pages;
using Front.Services;
using Shop_API.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Front
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        //var cart = CartService _cartService;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Product>>("https://localhost:7239/api/Products");
        }
        public async Task<List<Order>> GetOrdersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Order>>("https://localhost:7239/api/Order");
        }

        public async Task<Product> GetProductDetailsAsync(int productId)
        {
            return await _httpClient.GetFromJsonAsync<Product>($"https://localhost:7239/api/Products/{productId}");
        }

        public async Task<bool> PostProductsAsync(List<Product> products, int userId)
        {
            //var requestUrl = $"https://localhost:7239/api/Orders?userID={userId}";
            //var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CartService.ApiKey);


            var response = await _httpClient.PostAsJsonAsync($"https://localhost:7239/api/Orders?userID={userId}", products);
            return response.IsSuccessStatusCode;
        }

        public async Task DeleteProductAsync(int productId)
        {
            await _httpClient.DeleteAsync($"https://localhost:7239/api/Products/{productId}");
        }
    }
}
