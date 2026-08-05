using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryApi.Models
{
    public class WithdrawalHistory
    {
        [Key]
        public int Id { get; set; }

        public int StockItemId { get; set; }

        [ForeignKey("StockItemId")]
        public Product? StockItem { get; set; }

        [Required]
        public string ItemName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        [Required]
        public string Purpose { get; set; } = string.Empty;

        [Required]
        public string RecipientType { get; set; } = string.Empty;

        public int? RecipientEntityId { get; set; }

        public string RecipientCode { get; set; } = string.Empty;

        public string RecipientName { get; set; } = string.Empty;

        public string? WithdrawnBy { get; set; }

        public DateTime WithdrawalDate { get; set; } = DateTime.UtcNow;
    }
}
