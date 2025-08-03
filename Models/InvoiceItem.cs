using System.ComponentModel.DataAnnotations;

namespace ErpApi.Models
{
    public class InvoiceItem
    {
        public int Id { get; set; }

        [Required]
        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }

        [Required]
        [MaxLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [MaxLength(50)]
        public string? Case { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal GST { get; set; }
    }
}