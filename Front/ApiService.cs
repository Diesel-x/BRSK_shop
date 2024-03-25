using Shop_API.Models;
using System.Net.Http.Json;

namespace Front
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Product>>("https://localhost:7239/api/Products");
        }

        public async Task<Product> GetProductDetailsAsync(int productId)
        {
            return await _httpClient.GetFromJsonAsync<Product>($"https://localhost:7239/api/Products/{productId}");
        }

        public async Task DeleteProductAsync(int productId)
        {
            await _httpClient.DeleteAsync($"https://localhost:7239/api/Products/{productId}");
        }
    }
}
