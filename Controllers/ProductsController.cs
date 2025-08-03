using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
///// cursor/develop-net-core-erp-backend-api-2f37
using ERPBackend.Data;
using ERPBackend.Models;
using Microsoft.EntityFrameworkCore;
=======
using Microsoft.EntityFrameworkCore;
using ERPBackend.Data;
using ERPBackend.Models;
using ERPBackend.DTOs;
///// erp

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

///// cursor/develop-net-core-erp-backend-api-2f37
        /// <summary>
        /// Get a paginated list of products
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10)</param>
        /// <param name="search">Search term for item name</param>
        /// <returns>Paginated list of products</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ProductDto>), 200)]
        public async Task<IActionResult> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var query = _context.Products.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ItemName.Contains(search) || 
                                       p.Category != null && p.Category.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var products = await query
                .OrderBy(p => p.ItemName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ItemName = p.ItemName,
                    StockQuantity = p.StockQuantity,
                    Case = p.Case,
                    Price = p.Price,
                    GST = p.GST,
                    Description = p.Description,
                    Category = p.Category,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                Products = products,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = totalPages
            });
        }

        /// <summary>
        /// Get a specific product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            var productDto = new ProductDto
            {
                Id = product.Id,
                ItemName = product.ItemName,
                StockQuantity = product.StockQuantity,
                Case = product.Case,
                Price = product.Price,
                GST = product.GST,
                Description = product.Description,
                Category = product.Category,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

            return Ok(productDto);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="createProductDto">Product creation details</param>
        /// <returns>Created product</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), 201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = new Product
            {
                ItemName = createProductDto.ItemName,
                StockQuantity = createProductDto.StockQuantity,
                Case = createProductDto.Case,
                Price = createProductDto.Price,
                GST = createProductDto.GST,
                Description = createProductDto.Description,
                Category = createProductDto.Category
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var productDto = new ProductDto
            {
                Id = product.Id,
                ItemName = product.ItemName,
                StockQuantity = product.StockQuantity,
                Case = product.Case,
                Price = product.Price,
                GST = product.GST,
                Description = product.Description,
                Category = product.Category,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="updateProductDto">Product update details</param>
        /// <returns>Updated product</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateProductDto.ItemName))
                product.ItemName = updateProductDto.ItemName;

            if (updateProductDto.StockQuantity.HasValue)
                product.StockQuantity = updateProductDto.StockQuantity.Value;

            if (!string.IsNullOrEmpty(updateProductDto.Case))
                product.Case = updateProductDto.Case;

            if (updateProductDto.Price.HasValue)
                product.Price = updateProductDto.Price.Value;

            if (updateProductDto.GST.HasValue)
                product.GST = updateProductDto.GST.Value;

            if (updateProductDto.Description != null)
                product.Description = updateProductDto.Description;

            if (updateProductDto.Category != null)
                product.Category = updateProductDto.Category;

            await _context.SaveChangesAsync();

            var productDto = new ProductDto
            {
                Id = product.Id,
                ItemName = product.ItemName,
                StockQuantity = product.StockQuantity,
                Case = product.Case,
                Price = product.Price,
                GST = product.GST,
                Description = product.Description,
                Category = product.Category,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

            return Ok(productDto);
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
=======
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ProductDto>>> GetProducts(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? category = null)
        {
            try
            {
                var query = _context.Products.AsQueryable();

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p => p.ItemName.Contains(search) || 
                                           p.SKU.Contains(search) || 
                                           p.Description.Contains(search));
                }

                // Apply category filter
                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(p => p.Category == category);
                }

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var products = await query
                    .OrderBy(p => p.ItemName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductDto
                    {
                        Id = p.Id,
                        ItemName = p.ItemName,
                        StockQuantity = p.StockQuantity,
                        Case = p.Case,
                        Price = p.Price,
                        GST = p.GST,
                        Description = p.Description,
                        Category = p.Category,
                        SKU = p.SKU,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt
                    })
                    .ToListAsync();

                var result = new PaginatedResult<ProductDto>
                {
                    Data = products,
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
                return StatusCode(500, new { message = "An error occurred while retrieving products", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .Where(p => p.Id == id)
                    .Select(p => new ProductDto
                    {
                        Id = p.Id,
                        ItemName = p.ItemName,
                        StockQuantity = p.StockQuantity,
                        Case = p.Case,
                        Price = p.Price,
                        GST = p.GST,
                        Description = p.Description,
                        Category = p.Category,
                        SKU = p.SKU,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the product", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
        {
            try
            {
                // Check if SKU already exists (if provided)
                if (!string.IsNullOrEmpty(createProductDto.SKU))
                {
                    if (await _context.Products.AnyAsync(p => p.SKU == createProductDto.SKU))
                    {
                        return BadRequest(new { message = "SKU already exists" });
                    }
                }

                var product = new Product
                {
                    ItemName = createProductDto.ItemName,
                    StockQuantity = createProductDto.StockQuantity,
                    Case = createProductDto.Case,
                    Price = createProductDto.Price,
                    GST = createProductDto.GST,
                    Description = createProductDto.Description,
                    Category = createProductDto.Category,
                    SKU = createProductDto.SKU
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                var productDto = new ProductDto
                {
                    Id = product.Id,
                    ItemName = product.ItemName,
                    StockQuantity = product.StockQuantity,
                    Case = product.Case,
                    Price = product.Price,
                    GST = product.GST,
                    Description = product.Description,
                    Category = product.Category,
                    SKU = product.SKU,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the product", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }

                // Check if SKU already exists (if provided and different from current)
                if (!string.IsNullOrEmpty(updateProductDto.SKU) && updateProductDto.SKU != product.SKU)
                {
                    if (await _context.Products.AnyAsync(p => p.SKU == updateProductDto.SKU))
                    {
                        return BadRequest(new { message = "SKU already exists" });
                    }
                }

                // Update only provided fields
                if (!string.IsNullOrEmpty(updateProductDto.ItemName))
                    product.ItemName = updateProductDto.ItemName;
                
                if (updateProductDto.StockQuantity.HasValue)
                    product.StockQuantity = updateProductDto.StockQuantity.Value;
                
                if (!string.IsNullOrEmpty(updateProductDto.Case))
                    product.Case = updateProductDto.Case;
                
                if (updateProductDto.Price.HasValue)
                    product.Price = updateProductDto.Price.Value;
                
                if (updateProductDto.GST.HasValue)
                    product.GST = updateProductDto.GST.Value;
                
                if (!string.IsNullOrEmpty(updateProductDto.Description))
                    product.Description = updateProductDto.Description;
                
                if (!string.IsNullOrEmpty(updateProductDto.Category))
                    product.Category = updateProductDto.Category;
                
                if (!string.IsNullOrEmpty(updateProductDto.SKU))
                    product.SKU = updateProductDto.SKU;

                await _context.SaveChangesAsync();

                var productDto = new ProductDto
                {
                    Id = product.Id,
                    ItemName = product.ItemName,
                    StockQuantity = product.StockQuantity,
                    Case = product.Case,
                    Price = product.Price,
                    GST = product.GST,
                    Description = product.Description,
                    Category = product.Category,
                    SKU = product.SKU,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };

                return Ok(productDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the product", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.InvoiceItems)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }

                // Check if product is used in any invoices
                if (product.InvoiceItems.Any())
                {
                    return BadRequest(new { message = "Cannot delete product that is used in invoices" });
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the product", error = ex.Message });
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<string>>> GetCategories()
        {
            try
            {
                var categories = await _context.Products
                    .Where(p => !string.IsNullOrEmpty(p.Category))
                    .Select(p => p.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving categories", error = ex.Message });
            }
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<List<ProductDto>>> GetLowStockProducts([FromQuery] int threshold = 10)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.StockQuantity <= threshold)
                    .OrderBy(p => p.StockQuantity)
                    .Select(p => new ProductDto
                    {
                        Id = p.Id,
                        ItemName = p.ItemName,
                        StockQuantity = p.StockQuantity,
                        Case = p.Case,
                        Price = p.Price,
                        GST = p.GST,
                        Description = p.Description,
                        Category = p.Category,
                        SKU = p.SKU,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving low stock products", error = ex.Message });
            }
//// erp
        }
    }
}