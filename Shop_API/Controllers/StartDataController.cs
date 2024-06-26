﻿using Dapper;
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
                    new Product { Name = "Игрушка", Cost = 1000, Count = 2},
                    new Product { Name = "Мороженное", Cost = 60, Count = 6 },
                    new Product { Name = "Набор посуды", Cost = 1500, Count = 9 }
                };
                _context.Product.AddRange(products);
                await _context.SaveChangesAsync();

                var hmac = new HMACSHA512();
                var users = new List<User>
                {
                    new User { Name = "Админ",
                        RoleId = roles.First(r => r.Name == "Администратор").Id,
                        Login = "admin",
                        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("admin")),
                        PasswordSalt = hmac.Key
                    },
                    new User { Name = "Баянов Дияз Гайсаевич",
                        RoleId = roles.First(r => r.Name == "Клиент").Id,
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
                    products[2],
                    products[2]
                };

                var sumOrder = orderComposition.Sum(product => product.Cost);

                var user = users.FirstOrDefault(u => u.Login == "user");
                if (user != null)
                {
                    var order = new Order
                    {
                        SumCost = sumOrder,
                        UserID = user.Id
                    };

                    _context.Order.Add(order);
                    await _context.SaveChangesAsync();
                    var orderProducts = orderComposition.Distinct().Select(p => new OrderBody { orderID = order.Id, productID = p.Id, productCount = orderComposition.Count(p2 => p2 == p) });

                    _context.OrderBodies.AddRange(orderProducts);
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
