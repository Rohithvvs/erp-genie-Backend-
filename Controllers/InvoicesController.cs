using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpApi.Data;
using ErpApi.Models;

namespace ErpApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public InvoicesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET /api/invoices?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _db.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Items)
                .AsQueryable();
            var total = await query.CountAsync();
            var invoices = await query
                .OrderByDescending(i => i.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Ok(new { total, page, pageSize, data = invoices });
        }

        // GET /api/invoices/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var invoice = await _db.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }

        // POST /api/invoices
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Invoice invoice)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Validate products and deduct stock
            foreach (var item in invoice.Items)
            {
                var product = await _db.Products.FirstOrDefaultAsync(p => p.ItemName == item.ItemName);
                if (product == null)
                {
                    return BadRequest($"Product '{item.ItemName}' not found");
                }
                if (product.StockQuantity < item.Quantity)
                {
                    return BadRequest($"Insufficient stock for '{item.ItemName}'");
                }
                product.StockQuantity -= item.Quantity;
            }

            // Calculate totals
            invoice.Total = invoice.Items.Sum(i => i.Price * i.Quantity * (1 + i.GST / 100));
            invoice.Date = DateTime.UtcNow;

            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = invoice.Id }, invoice);
        }

        // PUT /api/invoices/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Invoice updated)
        {
            if (id != updated.Id) return BadRequest();
            var existing = await _db.Invoices.Include(i => i.Items).FirstOrDefaultAsync(i => i.Id == id);
            if (existing == null) return NotFound();

            existing.CustomerId = updated.CustomerId;
            existing.Total = updated.Total;
            // For demo, we won't update items & stock here. In production, handle diff.

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}