using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApi.Models;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WithdrawalsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WithdrawalsController(AppDbContext context)
        {
            _context = context;
        }

        // GET withdrawals
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Withdrawals
                .Include(w => w.Product)
                .Include(w => w.User)
                .ToListAsync());
        }

        // WITHDRAW inventory
        [HttpPost]
        public async Task<IActionResult> Add(Withdrawal withdrawal)
        {
            var product = await _context.Products.FindAsync(withdrawal.ProductId);

            if (product == null)
                return NotFound("Product not found");

            if (product.Quantity < withdrawal.Quantity)
                return BadRequest("Not enough stock");

            product.Quantity -= withdrawal.Quantity;

            _context.Withdrawals.Add(withdrawal);

            await _context.SaveChangesAsync();

            return Ok(withdrawal);
        }
    }
}