using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApi.Models;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryController(AppDbContext context)
        {
            _context = context;
        }

        // WITHDRAW STOCK
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(
            [FromQuery] int productId,
            [FromQuery] int quantity,
            [FromQuery] string role)
        {
            if (role != "Staff" && role != "Admin")
                return Unauthorized("Only staff or admin can withdraw");

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return NotFound("Product not found");

            if (quantity <= 0)
                return BadRequest("Quantity must be greater than 0");

            if (product.Quantity < quantity)
                return BadRequest("Not enough stock");

            product.Quantity -= quantity;

            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // PURCHASE STOCK
        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase(
            [FromQuery] int productId,
            [FromQuery] int quantity)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return NotFound("Product not found");

            if (quantity <= 0)
                return BadRequest("Quantity must be greater than 0");

            product.Quantity += quantity;

            await _context.SaveChangesAsync();

            return Ok(product);
        }
    }
}