using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApi.Models;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login(User login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == login.Username &&
                u.Password == login.Password);

            if (user == null)
                return Unauthorized("Invalid username or password");

            return Ok(new
            {
                message = "Login successful",
                role = user.Role
            });
        }

        // REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            var existingUser =
                await _context.Users.FirstOrDefaultAsync(u =>
                    u.Username == user.Username);

            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok("Account created");
        }
    }
}