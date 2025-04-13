using ProductManagmentSystem.Domain.Entities;

namespace ProductManagmentSystem.Domain.RepositoryInterfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product> AddAsync(Product product);
        Task UpdateProductStockAsync(Product productStockUpdate);
        Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedProductsAsync(int pageNumber, int pageSize);
    }
}
