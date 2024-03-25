using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_API.Data;
using Shop_API.Models;
using System.Security.Cryptography;
using System.Text;

namespace Shop_API.Controllers
{
    public class StartDataController : Controller
    {
        private readonly DBContext _context;

        public StartDataController(DBContext context)
        {
            _context = context;
        }

        [HttpPost("CreateTestData")]
        public async Task<ActionResult> CreateTestData()
        {
            try
            {
                var roles = new List<Role>
                {
                    new Role { Name = "Администратор" },
                    new Role { Name = "Системный администатор" },
                    new Role { Name = "Клиент" }
                };
                _context.Role.AddRange(roles);
                await _context.SaveChangesAsync();

                var products = new List<Product>
                {
                    new Product { Name = "Красильников Роман", Cost = 100, Count = 2 },
                    new Product { Name = "Гойдов Искандер", Cost = 1000, Count = 6 },
                    new Product { Name = "Зверь Никиточка", Cost = 10, Count = 9 }
                };
                _context.Product.AddRange(products);
                await _context.SaveChangesAsync();

                var hmac = new HMACSHA512();
                var users = new List<User>
                {
                    new User { Name = "Админка",
                        Role = roles.First(r => r.Name == "Администратор"),
                        RoleId = roles.First(r => r.Name == "Администратор").Id,
                        Login = "admin",
                        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("admin")),
                        PasswordSalt = hmac.Key
                    },
                    new User { Name = "Клиентик",
                        RoleId = roles.First(r => r.Name == "Клиент").Id,
                        Role = roles.First(r => r.Name == "Клиент"),
                        Login = "user",
                        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("user")),
                        PasswordSalt = hmac.Key
                    }
                };
                _context.User.AddRange(users);
                await _context.SaveChangesAsync();

                var orderComposition = new List<Product>
                {
                    products[1],
                    products[2],
                    products[2]
                };

                var sumOrder = orderComposition.Sum(product => product.Cost);

                var user = users.FirstOrDefault(u => u.Login == "user");
                if (user != null)
                {
                    var order = new Order
                    {
                        Products = orderComposition,
                        SumCost = sumOrder,
                        User = user,
                        UserID = user.Id
                    };

                    _context.Order.Add(order);
                    await _context.SaveChangesAsync();
                }

                return Ok("Тестовые данные успешно созданы.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при создании тестовых данных: {ex.Message}");
            }
        }
        [HttpDelete("DeleteAllData")]
        public async Task<IActionResult> DeleteAllData()
        {
            try 
            {
                var tables = await _context.Database.GetDbConnection().QueryAsync<string>("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'");

                foreach (var table in tables)
                {
                    // Выполняем операцию удаления всех данных из каждой таблицы
                    await _context.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE \"{table}\" RESTART IDENTITY CASCADE");
                }

                return Ok("База данных очищена");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка удаления данных: {ex.Message}");
            }
        }
    }
}
