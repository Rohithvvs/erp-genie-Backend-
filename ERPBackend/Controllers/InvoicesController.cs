using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ERPBackend.Data;
using ERPBackend.DTOs;
using ERPBackend.Models;
using System.Security.Claims;

namespace ERPBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceResponse>> CreateInvoice(CreateInvoiceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get current user ID from JWT token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized();
            }

            // Validate customer exists
            var customer = await _context.Customers.FindAsync(request.CustomerId);
            if (customer == null)
            {
                return BadRequest(new { message = "Customer not found" });
            }

            // Validate products and check stock
            var invoiceItems = new List<InvoiceItem>();
            decimal subTotal = 0;
            decimal totalGST = 0;

            foreach (var itemRequest in request.Items)
            {
                var product = await _context.Products.FindAsync(itemRequest.ProductId);
                if (product == null)
                {
                    return BadRequest(new { message = $"Product with ID {itemRequest.ProductId} not found" });
                }

                if (product.StockQuantity < itemRequest.Quantity)
                {
                    return BadRequest(new { message = $"Insufficient stock for product {product.ItemName}" });
                }

                // Calculate item total
                var itemTotal = product.Price * itemRequest.Quantity;
                var itemGST = (itemTotal * product.GST) / 100;

                var invoiceItem = new InvoiceItem
                {
                    ProductId = product.Id,
                    ItemName = product.ItemName,
                    Quantity = itemRequest.Quantity,
                    Case = product.Case,
                    Price = product.Price,
                    GST = product.GST,
                    TotalPrice = itemTotal + itemGST
                };

                invoiceItems.Add(invoiceItem);
                subTotal += itemTotal;
                totalGST += itemGST;

                // Deduct stock
                product.StockQuantity -= itemRequest.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
            }

            var totalAmount = subTotal + totalGST;

            // Generate invoice number (you might want to implement a more sophisticated numbering system)
            var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            var invoice = new Invoice
            {
                InvoiceNumber = invoiceNumber,
                InvoiceDate = DateTime.UtcNow,
                CustomerId = request.CustomerId,
                UserId = userId,
                SubTotal = subTotal,
                TotalGST = totalGST,
                TotalAmount = totalAmount,
                Status = "Pending",
                Notes = request.Notes
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Add invoice items
            foreach (var item in invoiceItems)
            {
                item.InvoiceId = invoice.Id;
            }
            _context.InvoiceItems.AddRange(invoiceItems);
            await _context.SaveChangesAsync();

            // Return the created invoice
            return await GetInvoice(invoice.Id);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedInvoicesResponse>> GetInvoices(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            // Get current user ID from JWT token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized();
            }

            var totalCount = await _context.Invoices
                .Where(i => i.UserId == userId)
                .CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var invoices = await _context.Invoices
                .Where(i => i.UserId == userId)
                .Include(i => i.Customer)
                .Include(i => i.InvoiceItems)
                .OrderByDescending(i => i.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var invoiceResponses = invoices.Select(i => new InvoiceResponse
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceDate = i.InvoiceDate,
                Customer = new CustomerResponse
                {
                    Id = i.Customer.Id,
                    Name = i.Customer.Name,
                    Address = i.Customer.Address,
                    PhoneNumber = i.Customer.PhoneNumber,
                    GST = i.Customer.GST,
                    Email = i.Customer.Email
                },
                SubTotal = i.SubTotal,
                TotalGST = i.TotalGST,
                TotalAmount = i.TotalAmount,
                Status = i.Status,
                Notes = i.Notes,
                Items = i.InvoiceItems.Select(ii => new InvoiceItemResponse
                {
                    Id = ii.Id,
                    ItemName = ii.ItemName,
                    Quantity = ii.Quantity,
                    Case = ii.Case,
                    Price = ii.Price,
                    GST = ii.GST,
                    TotalPrice = ii.TotalPrice
                }).ToList(),
                CreatedAt = i.CreatedAt
            }).ToList();

            var response = new PaginatedInvoicesResponse
            {
                Invoices = invoiceResponses,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceResponse>> GetInvoice(int id)
        {
            // Get current user ID from JWT token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized();
            }

            var invoice = await _context.Invoices
                .Where(i => i.Id == id && i.UserId == userId)
                .Include(i => i.Customer)
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                return NotFound();
            }

            var response = new InvoiceResponse
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                Customer = new CustomerResponse
                {
                    Id = invoice.Customer.Id,
                    Name = invoice.Customer.Name,
                    Address = invoice.Customer.Address,
                    PhoneNumber = invoice.Customer.PhoneNumber,
                    GST = invoice.Customer.GST,
                    Email = invoice.Customer.Email
                },
                SubTotal = invoice.SubTotal,
                TotalGST = invoice.TotalGST,
                TotalAmount = invoice.TotalAmount,
                Status = invoice.Status,
                Notes = invoice.Notes,
                Items = invoice.InvoiceItems.Select(ii => new InvoiceItemResponse
                {
                    Id = ii.Id,
                    ItemName = ii.ItemName,
                    Quantity = ii.Quantity,
                    Case = ii.Case,
                    Price = ii.Price,
                    GST = ii.GST,
                    TotalPrice = ii.TotalPrice
                }).ToList(),
                CreatedAt = invoice.CreatedAt
            };

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, UpdateInvoiceRequest request)
        {
            // Get current user ID from JWT token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized();
            }

            var invoice = await _context.Invoices
                .Where(i => i.Id == id && i.UserId == userId)
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                return NotFound();
            }

            if (request.Status != null)
                invoice.Status = request.Status;
            if (request.Notes != null)
                invoice.Notes = request.Notes;

            invoice.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}