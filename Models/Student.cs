using System.ComponentModel.DataAnnotations;

namespace InventoryApi.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;
    }
}
