

using Microsoft.EntityFrameworkCore;
using ProductManagmentSystem.Domain.Entities;
using ProductManagmentSystem.Infrastructure.Persistence;
using ProductManagmentSystem.Infrastructure.Repositories;

namespace ProductManagmentSystem.Tests.Repositories
{
    public class ProductRepositoryTests
    {
        private static ProductDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>().UseInMemoryDatabase(databaseName: dbName).Options;
            return new ProductDbContext(options);
        }
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var dbName = nameof(GetAllAsync_ShouldReturnAllProducts);
            using var context = CreateInMemoryContext(dbName);
            context.Products.AddRange(new List<Product>
        {
            new() { Id = 1, Name = "Prod1", ImageUrl = "http://image1", Price = 10, Description = "Desc1", StockQuantity = 5 },
            new() { Id = 2, Name = "Prod2", ImageUrl = "http://image2", Price = 20, Description = "Desc2", StockQuantity = 8 }
        });
            await context.SaveChangesAsync();

            var repository = new ProductRepository(context);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.Name == "Prod1");
            Assert.Contains(result, p => p.Name == "Prod2");
        }
        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var dbName = nameof(GetByIdAsync_ShouldReturnProduct_WhenProductExists);
            using var context = CreateInMemoryContext(dbName);
            var product = new Product { Id = 1, Name = "Prod1", ImageUrl = "http://image1", Price = 10, Description = "Desc", StockQuantity = 5 };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var repository = new ProductRepository(context);

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Prod1", result.Name);
        }
        [Fact]
        public async Task AddAsync_ShouldAddProductToDatabase()
        {
            // Arrange
            var dbName = nameof(AddAsync_ShouldAddProductToDatabase);
            using var context = CreateInMemoryContext(dbName);
            var repository = new ProductRepository(context);
            var newProduct = new Product { Name = "NewProd", ImageUrl = "http://newimage", Price = 30, Description = "New Desc", StockQuantity = 10 };

            // Act
            await repository.AddAsync(newProduct);

            // Assert
            var productInDb = await context.Products.FindAsync(1);
            Assert.NotNull(productInDb);
            Assert.Equal("NewProd", productInDb.Name);
            Assert.Equal("http://newimage", productInDb.ImageUrl);
            Assert.Equal(30, productInDb.Price);
            Assert.Equal("New Desc", productInDb.Description);
            Assert.Equal(10, productInDb.StockQuantity);
        }
        [Fact]
        public async Task UpdateProductStockAsync_ShouldUpdateStockAndKeepOtherProperties()
        {
            // Arrange
            var dbName = nameof(UpdateProductStockAsync_ShouldUpdateStockAndKeepOtherProperties);
            using var context = CreateInMemoryContext(dbName);
            var product = new Product { Id = 1, Name = "Prod1", ImageUrl = "http://image1", Price = 10, Description = "Desc", StockQuantity = 5 };
            context.Products.Add(product);
            await context.SaveChangesAsync();
            var repository = new ProductRepository(context);

            // Act
            product.StockQuantity = 20;
            await repository.UpdateProductStockAsync(product);

            // Assert
            var updatedProduct = await context.Products.FindAsync(1);
            Assert.Equal(20, updatedProduct?.StockQuantity);
            Assert.Equal("Prod1", updatedProduct?.Name);
            Assert.Equal("http://image1", updatedProduct?.ImageUrl);
            Assert.Equal(10, updatedProduct?.Price);
            Assert.Equal("Desc", updatedProduct?.Description);

        }
    }
}
