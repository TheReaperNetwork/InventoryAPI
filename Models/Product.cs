using System.ComponentModel.DataAnnotations;

namespace InventoryApi.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ItemCode { get; set; }

        [Required]
        public string ItemName { get; set; }

        public string Variant { get; set; }

        public decimal Cost { get; set; }

        public int Quantity { get; set; }

        public int MinimumStockLevel { get; set; } = 5;
    }
}