using ProductManagmentSystem.Contracts.DTOs;
using ProductManagmentSystem.Contracts.DTOs.Product;

namespace ProductManagmentSystem.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductDto dto);
        Task UpdateProductStockAsync(int id, int stockQuantity);
        Task<PagedResult<ProductDto>> GetPagedProductsAsync(int pageNumber, int pageSize);
    }
}
