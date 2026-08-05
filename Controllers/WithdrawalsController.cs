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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Withdrawals
                .Include(w => w.Product)
                .Include(w => w.User)
                .ToListAsync());
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] string? date, [FromQuery] string? item, [FromQuery] string? purpose, [FromQuery] string? recipientType, [FromQuery] string? recipientName, [FromQuery] string? search)
        {
            var query = _context.WithdrawalHistories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(date) && DateTime.TryParse(date, out var parsedDate))
            {
                query = query.Where(h => h.WithdrawalDate.Date == parsedDate.Date);
            }

            if (!string.IsNullOrWhiteSpace(item))
            {
                query = query.Where(h => h.ItemName.Contains(item));
            }

            if (!string.IsNullOrWhiteSpace(purpose))
            {
                query = query.Where(h => h.Purpose.Contains(purpose));
            }

            if (!string.IsNullOrWhiteSpace(recipientType))
            {
                query = query.Where(h => h.RecipientType == recipientType);
            }

            if (!string.IsNullOrWhiteSpace(recipientName))
            {
                query = query.Where(h => h.RecipientName.Contains(recipientName));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(h =>
                    h.RecipientName.Contains(term) ||
                    h.RecipientCode.Contains(term) ||
                    h.ItemName.Contains(term) ||
                    h.Purpose.Contains(term));
            }

            var history = await query
                .OrderByDescending(h => h.WithdrawalDate)
                .ToListAsync();

            return Ok(history);
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetStudents()
        {
            try
            {
                var students = await _context.Students
                    .OrderBy(s => s.FullName)
                    .ToListAsync();

                return Ok(students);
            }
            catch (Exception ex)
            {
                return Problem("Failed to load students: " + ex.Message);
            }
        }

        [HttpGet("instructors")]
        public async Task<IActionResult> GetInstructors()
        {
            try
            {
                var instructors = await _context.Instructors
                    .OrderBy(i => i.FullName)
                    .ToListAsync();

                return Ok(instructors);
            }
            catch (Exception ex)
            {
                return Problem("Failed to load instructors: " + ex.Message);
            }
        }

        private static List<Student> GetFallbackStudents()
        {
            return new List<Student>
            {
                new Student { Id = 1, StudentId = "STU0001", FullName = "John Smith" },
                new Student { Id = 2, StudentId = "STU0002", FullName = "Emily Johnson" },
                new Student { Id = 3, StudentId = "STU0003", FullName = "Michael Brown" },
                new Student { Id = 4, StudentId = "STU0004", FullName = "Sophia Davis" },
                new Student { Id = 5, StudentId = "STU0005", FullName = "Daniel Wilson" },
                new Student { Id = 6, StudentId = "STU0006", FullName = "Olivia Martinez" }
            };
        }

        private static List<Instructor> GetFallbackInstructors()
        {
            return new List<Instructor>
            {
                new Instructor { Id = 1, InstructorId = "INS0001", FullName = "Sarah Jones" },
                new Instructor { Id = 2, InstructorId = "INS0002", FullName = "David Lee" },
                new Instructor { Id = 3, InstructorId = "INS0003", FullName = "Megan Clark" },
                new Instructor { Id = 4, InstructorId = "INS0004", FullName = "James Walker" },
                new Instructor { Id = 5, InstructorId = "INS0005", FullName = "Rachel Hall" }
            };
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] WithdrawalRequest request)
        {
            if (request.Quantity <= 0)
                return BadRequest("Quantity must be greater than 0");

            if (string.IsNullOrWhiteSpace(request.Purpose))
                return BadRequest("Purpose is required");

            if (string.IsNullOrWhiteSpace(request.RecipientType))
                return BadRequest("Recipient type is required");

            if (request.RecipientType != "Student" && request.RecipientType != "Instructor")
                return BadRequest("Recipient type must be Student or Instructor");

            if (request.RecipientEntityId <= 0)
                return BadRequest("Recipient is required");

            var product = await _context.Products.FindAsync(request.ProductId);

            if (product == null)
                return NotFound("Product not found");

            if (product.Quantity < request.Quantity)
                return BadRequest("Not enough stock");

            Student? student = null;
            Instructor? instructor = null;

            if (request.RecipientType == "Student")
            {
                student = await _context.Students.FindAsync(request.RecipientEntityId);
                if (student == null)
                    return BadRequest("Recipient not found");
            }
            else
            {
                instructor = await _context.Instructors.FindAsync(request.RecipientEntityId);
                if (instructor == null)
                    return BadRequest("Recipient not found");
            }

            var recipientCode = request.RecipientType == "Student"
                ? student!.StudentId
                : instructor!.InstructorId;

            var recipientName = request.RecipientType == "Student"
                ? student!.FullName
                : instructor!.FullName;

            var withdrawnBy = Request.Headers["X-User-Name"].ToString();
            if (string.IsNullOrWhiteSpace(withdrawnBy))
            {
                withdrawnBy = "System";
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                product.Quantity -= request.Quantity;

                _context.Withdrawals.Add(new Withdrawal
                {
                    ProductId = request.ProductId,
                    UserId = request.UserId,
                    Quantity = request.Quantity,
                    WithdrawDate = DateTime.UtcNow
                });

                _context.WithdrawalHistories.Add(new WithdrawalHistory
                {
                    StockItemId = request.ProductId,
                    ItemName = product.ItemName,
                    Quantity = request.Quantity,
                    Purpose = request.Purpose,
                    RecipientType = request.RecipientType,
                    RecipientEntityId = request.RecipientEntityId,
                    RecipientCode = recipientCode,
                    RecipientName = recipientName,
                    WithdrawnBy = withdrawnBy,
                    WithdrawalDate = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Withdrawal successful",
                    product,
                    recipient = new { type = request.RecipientType, id = recipientCode, name = recipientName }
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public class WithdrawalRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string RecipientType { get; set; } = string.Empty;
        public int RecipientEntityId { get; set; }
        public int UserId { get; set; }
    }
}