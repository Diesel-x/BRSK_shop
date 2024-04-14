using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_API.Data;
using Shop_API.Models;

namespace Shop_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderBodiesController : ControllerBase
    {
        private readonly DBContext _context;

        public OrderBodiesController(DBContext context)
        {
            _context = context;
        }

        // GET: api/OrderBodies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderBody>>> GetOrderBodies()
        {
            return await _context.OrderBodies.ToListAsync();
        }

        // GET: api/Orders/userorders
        [HttpGet("userbodys")]
        public async Task<ActionResult<IEnumerable<OrderBody>>> GetUserBodys(int orderID)
        {
            var ordersbodys = await _context.OrderBodies.Where(o => o.orderID == orderID).ToListAsync();
            return ordersbodys;
        }

        // GET: api/OrderBodies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderBody>> GetOrderBody(int id)
        {
            var orderBody = await _context.OrderBodies.FindAsync(id);

            if (orderBody == null)
            {
                return NotFound();
            }

            return orderBody;
        }

        // PUT: api/OrderBodies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderBody(int id, OrderBody orderBody)
        {
            if (id != orderBody.Id)
            {
                return BadRequest();
            }

            _context.Entry(orderBody).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderBodyExists(id))
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

        // POST: api/OrderBodies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderBody>> PostOrderBody(OrderBody orderBody)
        {
            _context.OrderBodies.Add(orderBody);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderBody", new { id = orderBody.Id }, orderBody);
        }

        // DELETE: api/OrderBodies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderBody(int id)
        {
            var orderBody = await _context.OrderBodies.FindAsync(id);
            if (orderBody == null)
            {
                return NotFound();
            }

            _context.OrderBodies.Remove(orderBody);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderBodyExists(int id)
        {
            return _context.OrderBodies.Any(e => e.Id == id);
        }
    }
}
