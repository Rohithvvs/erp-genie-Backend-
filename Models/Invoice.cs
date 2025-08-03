using System.ComponentModel.DataAnnotations;

namespace ErpApi.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        [Range(0, double.MaxValue)]
        public decimal Total { get; set; }
    }
}