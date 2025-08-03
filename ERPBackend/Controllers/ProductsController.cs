using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ERPBackend.Data;
using ERPBackend.DTOs;
using ERPBackend.Models;

namespace ERPBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponse>> CreateProduct(CreateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = new Product
            {
                ItemName = request.ItemName,
                StockQuantity = request.StockQuantity,
                Case = request.Case,
                Price = request.Price,
                GST = request.GST,
                Description = request.Description
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var response = new ProductResponse
            {
                Id = product.Id,
                ItemName = product.ItemName,
                StockQuantity = product.StockQuantity,
                Case = product.Case,
                Price = product.Price,
                GST = product.GST,
                Description = product.Description,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, response);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedProductsResponse>> GetProducts(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var totalCount = await _context.Products.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var products = await _context.Products
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var productResponses = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                ItemName = p.ItemName,
                StockQuantity = p.StockQuantity,
                Case = p.Case,
                Price = p.Price,
                GST = p.GST,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();

            var response = new PaginatedProductsResponse
            {
                Products = productResponses,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var response = new ProductResponse
            {
                Id = product.Id,
                ItemName = product.ItemName,
                StockQuantity = product.StockQuantity,
                Case = product.Case,
                Price = product.Price,
                GST = product.GST,
                Description = product.Description,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductRequest request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (request.ItemName != null)
                product.ItemName = request.ItemName;
            if (request.StockQuantity.HasValue)
                product.StockQuantity = request.StockQuantity.Value;
            if (request.Case != null)
                product.Case = request.Case;
            if (request.Price.HasValue)
                product.Price = request.Price.Value;
            if (request.GST.HasValue)
                product.GST = request.GST.Value;
            if (request.Description != null)
                product.Description = request.Description;

            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Check if product is used in any invoices
            var hasInvoices = await _context.InvoiceItems.AnyAsync(ii => ii.ProductId == id);
            if (hasInvoices)
            {
                return BadRequest(new { message = "Cannot delete product that is used in invoices" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}