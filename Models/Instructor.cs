using System.ComponentModel.DataAnnotations;

namespace InventoryApi.Models
{
    public class Instructor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string InstructorId { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;
    }
}
