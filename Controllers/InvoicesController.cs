using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPBackend.Data;
using ERPBackend.Models;
using ERPBackend.Services;
using System.Security.Claims;

namespace ERPBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
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
        }
    }
}