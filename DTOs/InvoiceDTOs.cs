using System.ComponentModel.DataAnnotations;

namespace ERPBackend.DTOs
{
    public class CreateInvoiceDto
    {
        [Required]
        public CustomerDto Customer { get; set; } = null!;

        [Required]
        public List<InvoiceItemDto> Items { get; set; } = new();

        public DateTime? DueDate { get; set; }

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdateInvoiceDto
    {
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
    }

    public class InvoiceDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public CustomerDto Customer { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalGST { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public List<InvoiceItemDto> Items { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class InvoiceItemDto
    {
        public int ProductId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Case { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal GST { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string GST { get; set; } = string.Empty;
    }

    public class PaginatedResult<T>
    {
        public List<T> Data { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}