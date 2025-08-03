using System.ComponentModel.DataAnnotations;
///////////// cursor/develop-net-core-erp-backend-api-2f37
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPBackend.Models
{
    [Table("Customers")]
    public class Customer
    {
        [Key]
=======

namespace ERPBackend.Models
{
    public class Customer
    {
///////////////////////// erp
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

//////////////////// cursor/develop-net-core-erp-backend-api-2f37
        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(15)]
        public string? GST { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? GST { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateCustomerDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(15)]
        public string? GST { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }
=======
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
//////////////////////// erp
    }
}