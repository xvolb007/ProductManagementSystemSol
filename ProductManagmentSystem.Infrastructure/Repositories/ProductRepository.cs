using Microsoft.EntityFrameworkCore;
using ProductManagmentSystem.Domain.Entities;
using ProductManagmentSystem.Domain.RepositoryInterfaces;
using ProductManagmentSystem.Infrastructure.Persistence;

namespace ProductManagmentSystem.Infrastructure.Repositories
{
    public class ProductRepository: IProductRepository
    {
        private readonly ProductDbContext _productContext;

        public ProductRepository(ProductDbContext productContext)
        {
            _productContext = productContext;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _productContext.Products.ToListAsync();

        public async Task<Product?> GetByIdAsync(int id) =>
             await _productContext.Set<Product>().AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);

        public async Task<Product> AddAsync(Product product)
        {
            await _productContext.Products.AddAsync(product);
            await _productContext.SaveChangesAsync();
            return product;
        }

        public async Task UpdateProductStockAsync(Product productStockUpdate)
        {
            _productContext.Products.Update(productStockUpdate);
            await _productContext.SaveChangesAsync();
        }
        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedProductsAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _productContext.Products.CountAsync();
            var items = await _productContext.Products
                            .OrderBy(p => p.Id)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();
            return (items, totalCount);
        }
    }
}
