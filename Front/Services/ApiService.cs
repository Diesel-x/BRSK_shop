using Front.Pages;
using Microsoft.AspNetCore.Components;
using Shop_API.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace Front.Services
{
    public class ApiService
    {
        private NavigationManager _navigationManager;
        private readonly HttpClient _httpClient;

        //var cart = CartService _cartService;

        public ApiService(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
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

        public async Task<User> GetUser(string login)
        {
            var users = await _httpClient.GetFromJsonAsync<List<User>>($"https://localhost:7239/api/Users");
            var user = users.FirstOrDefault(x => x.Login == login);
            if (user != null)
            {
                return user;
            }
            return user;
        }

        public async Task<List<User>> GetUsers()
        { 
            return await _httpClient.GetFromJsonAsync<List<User>>($"https://localhost:7239/api/Users");
        }


        public async Task<List<Order>> GetUserOrders(string login)
        {
            return await _httpClient.GetFromJsonAsync<List<Order>>($"https://localhost:7239/api/Orders/userorders?login={login}");
        }

        public async Task<List<OrderBody>> GetUserOrderBodys(int orderId)
        {
            return await _httpClient.GetFromJsonAsync<List<OrderBody>>($"https://localhost:7239/api/OrderBodies/userbodys?orderID={orderId}");
        }

        public async Task<List<OrderBody>> GetOrderBodys()
        {
            return await _httpClient.GetFromJsonAsync<List<OrderBody>>($"https://localhost:7239/api/OrderBodies");
        }

        public async Task<string> PostProductsAsync(List<Product> products, int userId)
        {
            var response = await _httpClient.PostAsJsonAsync($"https://localhost:7239/api/Orders?userID={userId}", products);
            if (response.IsSuccessStatusCode)
            {
                
                CartService cartService = new CartService();
                cartService.CartItems = new List<Product>();
                _navigationManager.NavigateTo("/orderspage", true);
                return "Заказ создан";
            }
            return "Ошибка";
        }

        public async Task<string> PostAddProduct(string name, int cost)
        {
            var product = new Product
            {
                Name = name,
                Cost = cost,
                Count = 1
            };

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7239/api/Products", product);
            if (response.IsSuccessStatusCode)
            {
                return "Продукт создан";
            }
            return "Ошибка";
        }


        public async Task DeleteProductAsync(int productId)
        {
            await _httpClient.DeleteAsync($"https://localhost:7239/api/Products/{productId}");
        }
    }
}
