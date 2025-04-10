using Microsoft.AspNetCore.Mvc;
using ProductManagmentSystem.Application.Interfaces;
using ProductManagmentSystem.Contracts.DTOs.Product;

namespace ProductManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)} error occurred at {nameof(GetAll)} action.");
                return StatusCode(500, $"Error retrieving products: {ex.Message}");
            }

        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id < 0)
                return BadRequest("Product ID must be a non-negative integer.");
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return product == null ? NotFound() : Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)} error occurred at {nameof(GetById)} action.");
                return StatusCode(500, $"Error retrieving product ID: {ex.Message}");
            }

        }

        /// <summary>
        /// Create new product (only name and image URL required), rest of the attributes should be separated by comma, last attribute always without comma
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            try
            {
                var product = await _productService.CreateProductAsync(dto);
                return StatusCode(201, $"Product with name: {dto.Name} was appended to the database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)} error occurred at {nameof(Create)} action.");
                return StatusCode(500, $"Error creating product: {ex.Message}");
            }

        }

        /// <summary>
        /// Update product stock only
        /// </summary>
        [HttpPatch("{productId:int}/stock")]
        public async Task<IActionResult> UpdateStock(int productId, int stockQuantity)
        {
            try
            {
                await _productService.UpdateProductStockAsync(productId, stockQuantity);
                return Ok($"Stock updated for product ID: {productId}");
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Product with ID {productId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)} error occurred at {nameof(UpdateStock)}.");
                return StatusCode(500, "Internal server error while updating stock.");
            }
        }
    }
}
