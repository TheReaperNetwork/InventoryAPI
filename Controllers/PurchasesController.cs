using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApi.Models;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PurchasesController(AppDbContext context)
        {
            _context = context;
        }

        // GET purchases
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Purchases
                .Include(p => p.Product)
                .Include(p => p.User)
                .ToListAsync());
        }

        // RECORD purchase
        [HttpPost]
        public async Task<IActionResult> Add(Purchase purchase)
        {
            var product = await _context.Products.FindAsync(purchase.ProductId);

            if (product == null)
                return NotFound("Product not found");

            product.Quantity += purchase.Quantity;

            _context.Purchases.Add(purchase);

            await _context.SaveChangesAsync();

            return Ok(purchase);
        }
    }
}