using System.ComponentModel.DataAnnotations;

namespace ERPBackend.DTOs
{
    public class CreateProductRequest
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
    }

    public class UpdateProductRequest
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
    }

    public class ProductResponse
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string Case { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal GST { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PaginatedProductsResponse
    {
        public List<ProductResponse> Products { get; set; } = new List<ProductResponse>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}