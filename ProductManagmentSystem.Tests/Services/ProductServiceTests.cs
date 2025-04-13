

using AutoMapper;
using FakeItEasy;
using ProductManagmentSystem.Application.Interfaces;
using ProductManagmentSystem.Application.Mapping;
using ProductManagmentSystem.Application.Services;
using ProductManagmentSystem.Contracts.DTOs.Product;
using ProductManagmentSystem.Domain.Entities;
using ProductManagmentSystem.Domain.RepositoryInterfaces;

namespace ProductManagmentSystem.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly IProductRepository _productRepositoryFake;
        private readonly IMapper _mapper;
        private readonly ProductService _service;
        public ProductServiceTests()
        {
            _productRepositoryFake = A.Fake<IProductRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ProductProfile());
            });
            _mapper = config.CreateMapper();

            _service = new ProductService(_productRepositoryFake, _mapper);
        }
        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnMappedProductDtos()
        {
            // Arrange
            var products = new List<Product>
        {
            new() { Id = 1, Name = "Prod1", ImageUrl = "http://image1", Price = 10, Description = "Desc1", StockQuantity = 5 },
            new() { Id = 2, Name = "Prod2", ImageUrl = "http://image2", Price = 20, Description = "Desc2", StockQuantity = 8 }
        };
            A.CallTo(() => _productRepositoryFake.GetAllAsync())
                .Returns(Task.FromResult<IEnumerable<Product>>(products));

            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            var prod1 = result.First(p => p.Id == 1);
            Assert.Equal("Prod1", prod1.Name);
            Assert.Equal("http://image1", prod1.ImageUrl);
            Assert.Equal(10m, prod1.Price);
            Assert.Equal("Desc1", prod1.Description);
            Assert.Equal(5, prod1.StockQuantity);

            var prod2 = result.First(p => p.Id == 2);
            Assert.Equal("Prod2", prod2.Name);
            Assert.Equal("http://image2", prod2.ImageUrl);
            Assert.Equal(20m, prod2.Price);
            Assert.Equal("Desc2", prod2.Description);
            Assert.Equal(8, prod2.StockQuantity);
        }
        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnMappedProductDto_WhenProductExists()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Prod1", ImageUrl = "http://image1", Price = 10, Description = "Desc", StockQuantity = 5 };
            A.CallTo(() => _productRepositoryFake.GetByIdAsync(1))
                .Returns(Task.FromResult<Product?>(product));

            // Act
            var result = await _service.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Prod1", result.Name);
            Assert.Equal("http://image1", result.ImageUrl);
            Assert.Equal(10m, result.Price);
            Assert.Equal("Desc", result.Description);
            Assert.Equal(5, result.StockQuantity);
        }
        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            A.CallTo(() => _productRepositoryFake.GetByIdAsync(1))
                .Returns(Task.FromResult<Product?>(null));

            // Act
            var result = await _service.GetProductByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldMapAndReturnProductDto()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "NewProd",
                ImageUrl = "http://newimage",
                Price = 30,
                Description = "New Desc",
                StockQuantity = 10
            };

            A.CallTo(() => _productRepositoryFake.AddAsync(A<Product>.Ignored))
                .ReturnsLazily((Product p) => Task.FromResult(p));

            // Act
            var result = await _service.CreateProductAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("NewProd", result.Name);
            Assert.Equal("http://newimage", result.ImageUrl);
            Assert.Equal(30m, result.Price);
            Assert.Equal("New Desc", result.Description);
            Assert.Equal(10, result.StockQuantity);
        }

        [Fact]
        public async Task UpdateProductStockAsync_ShouldUpdateStock_WhenProductExists()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Prod1", ImageUrl = "http://image1", Price = 10, Description = "Desc", StockQuantity = 5 };
            A.CallTo(() => _productRepositoryFake.GetByIdAsync(1))
                .Returns(Task.FromResult<Product?>(product));

            var stockUpdateDto =  25;

            // Act
            await _service.UpdateProductStockAsync(1, stockUpdateDto);

            A.CallTo(() => _productRepositoryFake.UpdateProductStockAsync(
                    A<Product>.That.Matches(p => p.Id == 1 && p.StockQuantity == 25)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateProductStockAsync_ShouldThrowKeyNotFoundException_WhenProductNotFound()
        {
            // Arrange
            A.CallTo(() => _productRepositoryFake.GetByIdAsync(1))
                .Returns(Task.FromResult<Product?>(null));
            var stockUpdateDto = 25;

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateProductStockAsync(1, stockUpdateDto));
        }
        [Fact]
        public async Task GetPagedProductsAsync_ShouldReturnCorrectPagedResult()
        {
            // Arrange
            var products = Enumerable.Range(1, 25).Select(i => new Product
            {
                Id = i,
                Name = $"Product {i}",
                ImageUrl = $"http://image{i}",
                Price = i * 10,
                Description = $"Description {i}",
                StockQuantity = i * 5
            }).ToList();

            A.CallTo(() => _productRepositoryFake.GetPagedProductsAsync(A<int>.Ignored, A<int>.Ignored))
                .ReturnsLazily((int pageNumber, int pageSize) =>
                {
                    var ordered = products.OrderBy(p => p.Id);
                    var pageItems = ordered.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    return Task.FromResult<(IEnumerable<Product> Items, int TotalCount)>((pageItems, products.Count));
                });

            // Act
            var result = await _service.GetPagedProductsAsync(pageNumber: 3, pageSize: 10);

            // Assert
            Assert.Equal(25, result.TotalCount);
            Assert.Equal(3, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(5, result.Items.Count());
            Assert.Equal(21, result.Items.First().Id);
        }
    }
}
