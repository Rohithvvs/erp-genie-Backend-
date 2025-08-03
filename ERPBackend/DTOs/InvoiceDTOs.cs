using System.ComponentModel.DataAnnotations;

namespace ERPBackend.DTOs
{
    public class CreateInvoiceRequest
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public List<InvoiceItemRequest> Items { get; set; } = new List<InvoiceItemRequest>();

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class InvoiceItemRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class InvoiceResponse
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public CustomerResponse Customer { get; set; } = new CustomerResponse();
        public decimal SubTotal { get; set; }
        public decimal TotalGST { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public List<InvoiceItemResponse> Items { get; set; } = new List<InvoiceItemResponse>();
        public DateTime CreatedAt { get; set; }
    }

    public class InvoiceItemResponse
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Case { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal GST { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CustomerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? GST { get; set; }
        public string? Email { get; set; }
    }

    public class UpdateInvoiceRequest
    {
        [StringLength(20)]
        public string? Status { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class PaginatedInvoicesResponse
    {
        public List<InvoiceResponse> Invoices { get; set; } = new List<InvoiceResponse>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}