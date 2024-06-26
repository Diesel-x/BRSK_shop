﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_API.Data;
using Shop_API.Models;

namespace Shop_API.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DBContext _context;

        public OrdersController(DBContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Order.AsNoTracking().ToListAsync();
        }


        // GET: api/Orders/userorders
        [HttpGet("userorders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders(string login)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Login == login);
            if (user == null)
            {
                return NotFound($"User with login '{login}' not found.");
            }

            var userId = user.Id;
            var orders = await _context.Order.Where(o => o.UserID == userId).ToListAsync();
            return orders;
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Order.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }



        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder([FromBody]List<Product> products, int userID)
        {
            var order = new Order
            {
                SumCost = products.Sum(products => products.Cost),
                UserID = userID
            };
            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            //var orderProducts = products.Distinct().Select(p => new OrderBody { orderID = order.Id, productID = p.Id, productCount = products.Count(p2 => p2 == p) });

            var orderProducts = products
                .GroupBy(p => p.Id)
                .Select(group => new OrderBody
                {
                    orderID = order.Id,
                    productID = group.Key, 
                    productCount = group.Count() 
                });

            _context.OrderBodies.AddRange(orderProducts);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}
