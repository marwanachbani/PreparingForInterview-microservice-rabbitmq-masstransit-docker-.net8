using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;

        public OrdersController(OrderDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Order>> PlaceOrder(Order order)
        {
            // Adds the order along with its items to the context
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)  // Include related OrderItems
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            return order;
        }
    }
}
