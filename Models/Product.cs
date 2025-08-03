using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPBackend.Models
{
//// cursor/develop-net-core-erp-backend-api-2f37
    [Table("Products")]
    public class Product
    {
        [Key]
=======
    public class Product
    {
//// erp
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [Required]
//// cursor/develop-net-core-erp-backend-api-2f37
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Required]
=======
        public int StockQuantity { get; set; }

//// erp
        [StringLength(50)]
        public string Case { get; set; } = string.Empty;

        [Required]
//// cursor/develop-net-core-erp-backend-api-2f37
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 100)]
=======
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

///// erp
        [Column(TypeName = "decimal(5,2)")]
        public decimal GST { get; set; }

        [StringLength(500)]
///// cursor/develop-net-core-erp-backend-api-2f37
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string Case { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal GST { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateProductDto
    {
        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Required]
        [StringLength(50)]
        public string Case { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal GST { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }
    }

    public class UpdateProductDto
    {
        [StringLength(100)]
        public string? ItemName { get; set; }

        [Range(0, int.MaxValue)]
        public int? StockQuantity { get; set; }

        [StringLength(50)]
        public string? Case { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        [Range(0, 100)]
        public decimal? GST { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }
=======
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
//// erp
    }
}