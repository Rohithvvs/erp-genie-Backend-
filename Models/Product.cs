using System.ComponentModel.DataAnnotations;

namespace ErpApi.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        public int StockQuantity { get; set; }

        [MaxLength(50)]
        public string? Case { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, 100)]
        public decimal GST { get; set; }
    }
}