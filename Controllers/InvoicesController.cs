using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
///// cursor/develop-net-core-erp-backend-api-2f37
using ERPBackend.Data;
using ERPBackend.Models;
using ERPBackend.Services;
using System.Security.Claims;
=======
using Microsoft.EntityFrameworkCore;
using ERPBackend.Data;
using ERPBackend.Models;
using ERPBackend.DTOs;
////////////////////// erp

namespace ERPBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
/////////////// cursor/develop-net-core-erp-backend-api-2f37
        private readonly IInvoiceService _invoiceService;
        private readonly ApplicationDbContext _context;

        public InvoicesController(IInvoiceService invoiceService, ApplicationDbContext context)
        {
            _invoiceService = invoiceService;
            _context = context;
        }

        /// <summary>
        /// Get a paginated list of invoices
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10)</param>
        /// <returns>Paginated list of invoices</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedInvoicesResponse), 200)]
        public async Task<IActionResult> GetInvoices(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await _invoiceService.GetInvoicesAsync(page, pageSize);
            return Ok(response);
        }

        /// <summary>
        /// Get a specific invoice by ID
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <returns>Invoice details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InvoiceDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public async Task<IActionResult> GetInvoice(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);

            if (invoice == null)
            {
                return NotFound(new { message = "Invoice not found" });
            }

            return Ok(invoice);
        }

        /// <summary>
        /// Create a new invoice
        /// </summary>
        /// <param name="createInvoiceDto">Invoice creation details</param>
        /// <returns>Created invoice</returns>
        [HttpPost]
        [ProducesResponseType(typeof(InvoiceDto), 201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto createInvoiceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Get current user ID from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var invoice = await _invoiceService.CreateInvoiceAsync(createInvoiceDto, userId);
                return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the invoice" });
            }
        }

        /// <summary>
        /// Update an existing invoice
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <param name="updateInvoiceDto">Invoice update details</param>
        /// <returns>Updated invoice</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(InvoiceDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] UpdateInvoiceDto updateInvoiceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var invoice = await _invoiceService.UpdateInvoiceAsync(id, updateInvoiceDto);

            if (invoice == null)
            {
                return NotFound(new { message = "Invoice not found" });
            }

            return Ok(invoice);
        }

        /// <summary>
        /// Delete an invoice
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var success = await _invoiceService.DeleteInvoiceAsync(id);

            if (!success)
            {
                return NotFound(new { message = "Invoice not found" });
            }

            return NoContent();
=======
        private readonly ApplicationDbContext _context;

        public InvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<InvoiceDto>>> GetInvoices(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var query = _context.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.InvoiceItems)
                    .ThenInclude(ii => ii.Product)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(i => i.Status == status);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(i => i.InvoiceDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(i => i.InvoiceDate <= toDate.Value);
                }

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var invoices = await query
                    .OrderByDescending(i => i.CreatedAt)
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
                            Address = i.Customer.Address,
                            PhoneNumber = i.Customer.PhoneNumber,
                            GST = i.Customer.GST
                        },
                        InvoiceDate = i.InvoiceDate,
                        DueDate = i.DueDate,
                        SubTotal = i.SubTotal,
                        TotalGST = i.TotalGST,
                        TotalAmount = i.TotalAmount,
                        Status = i.Status,
                        Notes = i.Notes,
                        Items = i.InvoiceItems.Select(ii => new InvoiceItemDto
                        {
                            ProductId = ii.ProductId,
                            ItemName = ii.ItemName,
                            Quantity = ii.Quantity,
                            Case = ii.Case,
                            Price = ii.Price,
                            GST = ii.GST,
                            LineTotal = ii.LineTotal
                        }).ToList(),
                        CreatedAt = i.CreatedAt
                    })
                    .ToListAsync();

                var result = new PaginatedResult<InvoiceDto>
                {
                    Data = invoices,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    HasNextPage = page < totalPages,
                    HasPreviousPage = page > 1
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving invoices", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
        {
            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.InvoiceItems)
                    .ThenInclude(ii => ii.Product)
                    .Where(i => i.Id == id)
                    .Select(i => new InvoiceDto
                    {
                        Id = i.Id,
                        InvoiceNumber = i.InvoiceNumber,
                        Customer = new CustomerDto
                        {
                            Id = i.Customer.Id,
                            Name = i.Customer.Name,
                            Address = i.Customer.Address,
                            PhoneNumber = i.Customer.PhoneNumber,
                            GST = i.Customer.GST
                        },
                        InvoiceDate = i.InvoiceDate,
                        DueDate = i.DueDate,
                        SubTotal = i.SubTotal,
                        TotalGST = i.TotalGST,
                        TotalAmount = i.TotalAmount,
                        Status = i.Status,
                        Notes = i.Notes,
                        Items = i.InvoiceItems.Select(ii => new InvoiceItemDto
                        {
                            ProductId = ii.ProductId,
                            ItemName = ii.ItemName,
                            Quantity = ii.Quantity,
                            Case = ii.Case,
                            Price = ii.Price,
                            GST = ii.GST,
                            LineTotal = ii.LineTotal
                        }).ToList(),
                        CreatedAt = i.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (invoice == null)
                {
                    return NotFound(new { message = "Invoice not found" });
                }

                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the invoice", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceDto>> CreateInvoice(CreateInvoiceDto createInvoiceDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create or find customer
                Customer? customer = null;
                if (createInvoiceDto.Customer.Id > 0)
                {
                    customer = await _context.Customers.FindAsync(createInvoiceDto.Customer.Id);
                    if (customer == null)
                    {
                        return BadRequest(new { message = "Customer not found" });
                    }
                }
                else
                {
                    customer = new Customer
                    {
                        Name = createInvoiceDto.Customer.Name,
                        Address = createInvoiceDto.Customer.Address,
                        PhoneNumber = createInvoiceDto.Customer.PhoneNumber,
                        GST = createInvoiceDto.Customer.GST
                    };
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                }

                // Validate stock availability and calculate totals
                decimal subTotal = 0;
                decimal totalGST = 0;
                var invoiceItems = new List<InvoiceItem>();

                foreach (var item in createInvoiceDto.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null)
                    {
                        return BadRequest(new { message = $"Product with ID {item.ProductId} not found" });
                    }

                    if (product.StockQuantity < item.Quantity)
                    {
                        return BadRequest(new { message = $"Insufficient stock for product {product.ItemName}. Available: {product.StockQuantity}, Requested: {item.Quantity}" });
                    }

                    var lineTotal = item.Quantity * item.Price;
                    var gstAmount = lineTotal * (item.GST / 100);
                    
                    subTotal += lineTotal;
                    totalGST += gstAmount;

                    invoiceItems.Add(new InvoiceItem
                    {
                        ProductId = item.ProductId,
                        ItemName = item.ItemName,
                        Quantity = item.Quantity,
                        Case = item.Case,
                        Price = item.Price,
                        GST = item.GST,
                        LineTotal = lineTotal + gstAmount
                    });

                    // Deduct stock
                    product.StockQuantity -= item.Quantity;
                }

                // Generate invoice number
                var invoiceNumber = await GenerateInvoiceNumber();

                // Create invoice
                var invoice = new Invoice
                {
                    InvoiceNumber = invoiceNumber,
                    CustomerId = customer.Id,
                    InvoiceDate = DateTime.UtcNow,
                    DueDate = createInvoiceDto.DueDate,
                    SubTotal = subTotal,
                    TotalGST = totalGST,
                    TotalAmount = subTotal + totalGST,
                    Status = "Pending",
                    Notes = createInvoiceDto.Notes,
                    InvoiceItems = invoiceItems
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return created invoice
                var invoiceDto = new InvoiceDto
                {
                    Id = invoice.Id,
                    InvoiceNumber = invoice.InvoiceNumber,
                    Customer = new CustomerDto
                    {
                        Id = customer.Id,
                        Name = customer.Name,
                        Address = customer.Address,
                        PhoneNumber = customer.PhoneNumber,
                        GST = customer.GST
                    },
                    InvoiceDate = invoice.InvoiceDate,
                    DueDate = invoice.DueDate,
                    SubTotal = invoice.SubTotal,
                    TotalGST = invoice.TotalGST,
                    TotalAmount = invoice.TotalAmount,
                    Status = invoice.Status,
                    Notes = invoice.Notes,
                    Items = invoiceItems.Select(ii => new InvoiceItemDto
                    {
                        ProductId = ii.ProductId,
                        ItemName = ii.ItemName,
                        Quantity = ii.Quantity,
                        Case = ii.Case,
                        Price = ii.Price,
                        GST = ii.GST,
                        LineTotal = ii.LineTotal
                    }).ToList(),
                    CreatedAt = invoice.CreatedAt
                };

                return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoiceDto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "An error occurred while creating the invoice", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InvoiceDto>> UpdateInvoice(int id, UpdateInvoiceDto updateInvoiceDto)
        {
            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.InvoiceItems)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (invoice == null)
                {
                    return NotFound(new { message = "Invoice not found" });
                }

                // Update only provided fields
                if (!string.IsNullOrEmpty(updateInvoiceDto.Status))
                    invoice.Status = updateInvoiceDto.Status;
                
                if (updateInvoiceDto.DueDate.HasValue)
                    invoice.DueDate = updateInvoiceDto.DueDate;
                
                if (!string.IsNullOrEmpty(updateInvoiceDto.Notes))
                    invoice.Notes = updateInvoiceDto.Notes;

                await _context.SaveChangesAsync();

                var invoiceDto = new InvoiceDto
                {
                    Id = invoice.Id,
                    InvoiceNumber = invoice.InvoiceNumber,
                    Customer = new CustomerDto
                    {
                        Id = invoice.Customer.Id,
                        Name = invoice.Customer.Name,
                        Address = invoice.Customer.Address,
                        PhoneNumber = invoice.Customer.PhoneNumber,
                        GST = invoice.Customer.GST
                    },
                    InvoiceDate = invoice.InvoiceDate,
                    DueDate = invoice.DueDate,
                    SubTotal = invoice.SubTotal,
                    TotalGST = invoice.TotalGST,
                    TotalAmount = invoice.TotalAmount,
                    Status = invoice.Status,
                    Notes = invoice.Notes,
                    Items = invoice.InvoiceItems.Select(ii => new InvoiceItemDto
                    {
                        ProductId = ii.ProductId,
                        ItemName = ii.ItemName,
                        Quantity = ii.Quantity,
                        Case = ii.Case,
                        Price = ii.Price,
                        GST = ii.GST,
                        LineTotal = ii.LineTotal
                    }).ToList(),
                    CreatedAt = invoice.CreatedAt
                };

                return Ok(invoiceDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the invoice", error = ex.Message });
            }
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetDashboardData()
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var currentMonth = new DateTime(today.Year, today.Month, 1);
                var lastMonth = currentMonth.AddMonths(-1);

                var dashboardData = new
                {
                    TotalInvoices = await _context.Invoices.CountAsync(),
                    PendingInvoices = await _context.Invoices.CountAsync(i => i.Status == "Pending"),
                    PaidInvoices = await _context.Invoices.CountAsync(i => i.Status == "Paid"),
                    TotalRevenue = await _context.Invoices
                        .Where(i => i.Status == "Paid")
                        .SumAsync(i => i.TotalAmount),
                    MonthlyRevenue = await _context.Invoices
                        .Where(i => i.Status == "Paid" && i.InvoiceDate >= currentMonth)
                        .SumAsync(i => i.TotalAmount),
                    LastMonthRevenue = await _context.Invoices
                        .Where(i => i.Status == "Paid" && i.InvoiceDate >= lastMonth && i.InvoiceDate < currentMonth)
                        .SumAsync(i => i.TotalAmount),
                    InvoicesToday = await _context.Invoices.CountAsync(i => i.InvoiceDate.Date == today),
                    InvoicesThisMonth = await _context.Invoices.CountAsync(i => i.InvoiceDate >= currentMonth)
                };

                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving dashboard data", error = ex.Message });
            }
        }

        private async Task<string> GenerateInvoiceNumber()
        {
            var today = DateTime.UtcNow;
            var prefix = $"INV-{today:yyyyMM}-";
            
            var lastInvoice = await _context.Invoices
                .Where(i => i.InvoiceNumber.StartsWith(prefix))
                .OrderByDescending(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            if (lastInvoice == null)
            {
                return $"{prefix}0001";
            }

            var lastNumber = lastInvoice.InvoiceNumber.Substring(prefix.Length);
            if (int.TryParse(lastNumber, out int number))
            {
                return $"{prefix}{(number + 1):D4}";
            }

            return $"{prefix}0001";
////erp
        }
    }
}