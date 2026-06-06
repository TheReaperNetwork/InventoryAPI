using Microsoft.AspNetCore.Mvc;
using InventoryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == user.Username);

            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(user);
        }
    }
}