using System.ComponentModel.DataAnnotations;

namespace ERPBackend.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Phone]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(15)]
        public string GST { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}