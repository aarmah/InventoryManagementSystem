using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using InventoryAPI.Models;
using InventoryAPI.DTOs;
using InventoryAPI.Repositories;
using System.Diagnostics;

namespace InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductRepository productRepository, IMapper mapper, ILogger<ProductsController> logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        //{
        //    var products = await _productRepository.GetProductsWithCategoryAsync();
        //    var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        //    return Ok(productDtos);
        //}

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var products = await _productRepository.GetProductsWithCategoryAsync();
                stopwatch.Stop();
                _logger.LogInformation($"GetProducts took {stopwatch.ElapsedMilliseconds}ms, returned {products.Count()} products");

                var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetProducts failed after {stopwatch.ElapsedMilliseconds}ms");
                throw;
            }
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productRepository.GetProductWithCategoryAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
        {
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productDtos);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<Product>(createProductDto);
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            var createdProduct = await _productRepository.AddAsync(product);
            var productDto = _mapper.Map<ProductDto>(createdProduct);

            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, productDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        //public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var existingProduct = await _productRepository.GetByIdAsync(id);
        //    if (existingProduct == null)
        //    {
        //        return NotFound();
        //    }

        //    _mapper.Map(updateProductDto, existingProduct);
        //    existingProduct.UpdatedAt = DateTime.UtcNow;

        //    await _productRepository.UpdateAsync(existingProduct);
        //    return NoContent();
        //}

        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
                Console.WriteLine($"UpdateProduct called for ID: {id}");
                Console.WriteLine($"Update data: Name={updateProductDto.Name}, Price={updateProductDto.Price}, Quantity={updateProductDto.Quantity}");

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingProduct = await _productRepository.GetByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                // Update the product properties
                existingProduct.Name = updateProductDto.Name;
                existingProduct.Description = updateProductDto.Description;
                existingProduct.Price = updateProductDto.Price;
                existingProduct.Quantity = updateProductDto.Quantity;
                existingProduct.CategoryId = updateProductDto.CategoryId;
                existingProduct.UpdatedAt = DateTime.UtcNow;

                await _productRepository.UpdateAsync(existingProduct);

                return Ok(new { message = "Product updated successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productRepository.DeleteAsync(product);
            return NoContent();
        }

        [HttpGet("paged")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductPaginationDto>> GetProductsPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var (products, totalCount) = await _productRepository.GetProductsPagedAsync(pageNumber, pageSize);
                var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

                var result = new ProductPaginationDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    Products = productDtos
                };

                stopwatch.Stop();
                _logger.LogInformation($"GetProductsPaged took {stopwatch.ElapsedMilliseconds}ms");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetProductsPaged failed after {stopwatch.ElapsedMilliseconds}ms");
                throw;
            }
        }
    }
}