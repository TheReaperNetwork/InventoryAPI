using Microsoft.AspNetCore.Mvc;
using System.Linq;
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

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                .Select(u => new { u.Id, u.Username, Role = u.Role ?? "User" })
                .ToListAsync();

            return Ok(users);
        }

        public class RoleUpdateDto
        {
            public string Role { get; set; } = "User";
        }

        // PUT: api/users/{id}/role
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleUpdateDto dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return NotFound("User not found");

            user.Role = dto.Role;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return Ok(new { user.Id, user.Username, Role = user.Role });
        }
    }
}