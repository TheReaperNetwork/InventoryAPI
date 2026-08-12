using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApi.Models;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET all products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.Products.ToListAsync();

            return Ok(products);
        }

        // GET product by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            return Ok(product);
        }

        // ADD product
        [HttpPost]
        public async Task<IActionResult> Add(Product product, string role)
        {
            if (role != "Admin")
            {
                return Unauthorized("Only Admin can add products");
            }

            if (string.IsNullOrWhiteSpace(product.ItemName))
            {
                return BadRequest("Item name is required");
            }

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // EDIT product
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, Product updatedProduct, string role)
        {
            // Allow Admin and Staff to edit products
            if (role != "Admin" && role != "Staff")
            {
                return Unauthorized("Only Admin or Staff can edit products");
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            product.ItemCode = updatedProduct.ItemCode;
            product.ItemName = updatedProduct.ItemName;
            product.Variant = updatedProduct.Variant;
            product.Cost = updatedProduct.Cost;
            product.Quantity = updatedProduct.Quantity;
            product.MinimumStockLevel = updatedProduct.MinimumStockLevel;

            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // DELETE product
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, string role)
        {
            if (role != "Admin")
            {
                return Unauthorized("Only Admin can delete products");
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return Ok("Product deleted successfully");
        }
    }
}