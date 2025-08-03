using ERPBackend.Data;
using ERPBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ERPBackend.Services
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createInvoiceDto, int userId);
        Task<PaginatedInvoicesResponse> GetInvoicesAsync(int page = 1, int pageSize = 10);
        Task<InvoiceDto?> GetInvoiceByIdAsync(int id);
        Task<InvoiceDto?> UpdateInvoiceAsync(int id, UpdateInvoiceDto updateInvoiceDto);
        Task<bool> DeleteInvoiceAsync(int id);
        string GenerateInvoiceNumber();
    }

    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;

        public InvoiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createInvoiceDto, int userId)
        {
            // Validate customer exists
            var customer = await _context.Customers.FindAsync(createInvoiceDto.CustomerId);
            if (customer == null)
            {
                throw new ArgumentException("Customer not found");
            }

            // Create invoice
            var invoice = new Invoice
            {
                InvoiceNumber = GenerateInvoiceNumber(),
                CustomerId = createInvoiceDto.CustomerId,
                UserId = userId,
                Notes = createInvoiceDto.Notes,
                Status = "Pending"
            };

            decimal subTotal = 0;
            decimal gstAmount = 0;

            // Process invoice items
            foreach (var itemDto in createInvoiceDto.Items)
            {
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null)
                {
                    throw new ArgumentException($"Product with ID {itemDto.ProductId} not found");
                }

                if (product.StockQuantity < itemDto.Quantity)
                {
                    throw new ArgumentException($"Insufficient stock for product {product.ItemName}");
                }

                // Calculate item total
                var itemTotal = product.Price * itemDto.Quantity;
                var itemGst = (itemTotal * product.GST) / 100;

                var invoiceItem = new InvoiceItem
                {
                    ProductId = product.Id,
                    ItemName = product.ItemName,
                    Quantity = itemDto.Quantity,
                    Case = product.Case,
                    Price = product.Price,
                    GST = product.GST,
                    TotalPrice = itemTotal + itemGst
                };

                invoice.InvoiceItems.Add(invoiceItem);

                // Update stock
                product.StockQuantity -= itemDto.Quantity;

                subTotal += itemTotal;
                gstAmount += itemGst;
            }

            invoice.SubTotal = subTotal;
            invoice.GSTAmount = gstAmount;
            invoice.TotalAmount = subTotal + gstAmount;

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return await GetInvoiceByIdAsync(invoice.Id) ?? throw new InvalidOperationException("Failed to create invoice");
        }

        public async Task<PaginatedInvoicesResponse> GetInvoicesAsync(int page = 1, int pageSize = 10)
        {
            var query = _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.InvoiceItems)
                .OrderByDescending(i => i.CreatedAt);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var invoices = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    Customer = new CustomerDto
                    {
                        Id = i.Customer.Id,
                        Name = i.Customer.Name,
                        PhoneNumber = i.Customer.PhoneNumber,
                        Address = i.Customer.Address,
                        GST = i.Customer.GST,
                        Email = i.Customer.Email,
                        CreatedAt = i.Customer.CreatedAt,
                        UpdatedAt = i.Customer.UpdatedAt
                    },
                    SubTotal = i.SubTotal,
                    GSTAmount = i.GSTAmount,
                    TotalAmount = i.TotalAmount,
                    Status = i.Status,
                    Notes = i.Notes,
                    InvoiceDate = i.InvoiceDate,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt,
                    InvoiceItems = i.InvoiceItems.Select(ii => new InvoiceItemDto
                    {
                        Id = ii.Id,
                        ItemName = ii.ItemName,
                        Quantity = ii.Quantity,
                        Case = ii.Case,
                        Price = ii.Price,
                        GST = ii.GST,
                        TotalPrice = ii.TotalPrice
                    }).ToList()
                })
                .ToListAsync();

            return new PaginatedInvoicesResponse
            {
                Invoices = invoices,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<InvoiceDto?> GetInvoiceByIdAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return null;

            return new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                Customer = new CustomerDto
                {
                    Id = invoice.Customer.Id,
                    Name = invoice.Customer.Name,
                    PhoneNumber = invoice.Customer.PhoneNumber,
                    Address = invoice.Customer.Address,
                    GST = invoice.Customer.GST,
                    Email = invoice.Customer.Email,
                    CreatedAt = invoice.Customer.CreatedAt,
                    UpdatedAt = invoice.Customer.UpdatedAt
                },
                SubTotal = invoice.SubTotal,
                GSTAmount = invoice.GSTAmount,
                TotalAmount = invoice.TotalAmount,
                Status = invoice.Status,
                Notes = invoice.Notes,
                InvoiceDate = invoice.InvoiceDate,
                CreatedAt = invoice.CreatedAt,
                UpdatedAt = invoice.UpdatedAt,
                InvoiceItems = invoice.InvoiceItems.Select(ii => new InvoiceItemDto
                {
                    Id = ii.Id,
                    ItemName = ii.ItemName,
                    Quantity = ii.Quantity,
                    Case = ii.Case,
                    Price = ii.Price,
                    GST = ii.GST,
                    TotalPrice = ii.TotalPrice
                }).ToList()
            };
        }

        public async Task<InvoiceDto?> UpdateInvoiceAsync(int id, UpdateInvoiceDto updateInvoiceDto)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                return null;

            if (!string.IsNullOrEmpty(updateInvoiceDto.Status))
            {
                invoice.Status = updateInvoiceDto.Status;
            }

            if (updateInvoiceDto.Notes != null)
            {
                invoice.Notes = updateInvoiceDto.Notes;
            }

            await _context.SaveChangesAsync();

            return await GetInvoiceByIdAsync(id);
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return false;

            // Restore stock for all items
            foreach (var item in invoice.InvoiceItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                }
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return true;
        }

        public string GenerateInvoiceNumber()
        {
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var random = new Random();
            var randomPart = random.Next(1000, 9999).ToString();
            return $"INV-{date}-{randomPart}";
        }
    }
}