
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductManagementSystem.API.Controllers;
using ProductManagmentSystem.Application.Interfaces;
using ProductManagmentSystem.Contracts.DTOs;
using ProductManagmentSystem.Contracts.DTOs.Product;

namespace ProductManagmentSystem.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly ProductController _controller;
        private readonly IProductService _productServiceFake;
        private readonly ILogger<ProductController> _loggerFake;

        public ProductControllerTests()
        {
            // Создаем fake объекты через FakeItEasy
            _productServiceFake = A.Fake<IProductService>();
            _loggerFake = A.Fake<ILogger<ProductController>>();
            _controller = new ProductController(_productServiceFake, _loggerFake);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResult_WithListOfProducts()
        {
            // Arrange
            var productDtos = new List<ProductDto>
        {
            new() { Name = "Prod1", ImageUrl = "http://image1", Price = 10, Description = "Desc", StockQuantity = 5 }
        };
            A.CallTo(() => _productServiceFake.GetAllProductsAsync())
                .Returns(Task.FromResult<IEnumerable<ProductDto>>(productDtos));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetById_ShouldReturnBadRequest_WhenIdNegative()
        {
            // Act
            var result = await _controller.GetById(-1);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            A.CallTo(() => _productServiceFake.GetProductByIdAsync(1))
                .Returns(Task.FromResult<ProductDto?>(null));

            // Act
            var result = await _controller.GetById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenProductExists()
        {
            // Arrange
            var productDto = new ProductDto { Id = 1, Name = "Prod1", ImageUrl = "http://image1", Price = 10, Description = "Desc", StockQuantity = 5 };
            A.CallTo(() => _productServiceFake.GetProductByIdAsync(1))
                .Returns(Task.FromResult<ProductDto?>(productDto));

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal(1, returnedDto.Id);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedStatus_WhenProductCreatedSuccessfully()
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

            A.CallTo(() => _productServiceFake.CreateProductAsync(createDto))
                .Returns(Task.FromResult(new ProductDto
                {
                    Id = 1,
                    Name = createDto.Name,
                    ImageUrl = createDto.ImageUrl,
                    Price = createDto.Price ?? 0,
                    Description = createDto.Description ?? string.Empty,
                    StockQuantity = createDto.StockQuantity ?? 0
                }));

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateStock_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int stockDto = 50;
            A.CallTo(() => _productServiceFake.UpdateProductStockAsync(1, stockDto))
                .ThrowsAsync(new KeyNotFoundException("Product not found."));

            // Act
            var result = await _controller.UpdateStock(1, stockDto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateStock_ShouldReturnOk_WhenUpdateSucceeds()
        {
            // Arrange
            var stockDto = 50;
            A.CallTo(() => _productServiceFake.UpdateProductStockAsync(1, stockDto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateStock(1, stockDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Stock updated", okResult?.Value?.ToString());
        }
        [Fact]
        public async Task GetPaged_ShouldReturnOkResult_WithAllFieldsCorrectlyPopulated()
        {
            // Arrange
            int pageNumber = 2;
            int pageSize = 5;

            var itemsOnSecondPage = new List<ProductDto>
            {
                new ProductDto {
                    Id = 6,
                    Name = "Prod6",
                    ImageUrl = "http://image6",
                    Price = 60m,
                    Description = "Description 6",
                    StockQuantity = 12
                },
                new ProductDto {
                    Id = 7,
                    Name = "Prod7",
                    ImageUrl = "http://image7",
                    Price = 70m,
                    Description = "Description 7",
                    StockQuantity = 14
                },
                new ProductDto {
                    Id = 8,
                    Name = "Prod8",
                    ImageUrl = "http://image8",
                    Price = 80m,
                    Description = "Description 8",
                    StockQuantity = 16
                },
                new ProductDto {
                    Id = 9,
                    Name = "Prod9",
                    ImageUrl = "http://image9",
                    Price = 90m,
                    Description = "Description 9",
                    StockQuantity = 18
                },
                new ProductDto {
                    Id = 10,
                    Name = "Prod10",
                    ImageUrl = "http://image10",
                    Price = 100m,
                    Description = "Description 10",
                    StockQuantity = 20
                }
            };

            var pagedResult = new PagedResult<ProductDto>(
                items: itemsOnSecondPage,
                totalCount: 15,
                pageNumber: pageNumber,
                pageSize: pageSize
            );

            A.CallTo(() => _productServiceFake.GetPagedProductsAsync(pageNumber, pageSize))
                .Returns(pagedResult);

            // Act
            var result = await _controller.GetPaged(pageNumber, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualPaged = Assert.IsType<PagedResult<ProductDto>>(okResult.Value);

            Assert.Equal(15, actualPaged.TotalCount);
            Assert.Equal(pageNumber, actualPaged.PageNumber);
            Assert.Equal(pageSize, actualPaged.PageSize);
            Assert.Equal(5, actualPaged.Items.Count());

            var firstProduct = actualPaged.Items.First();
            Assert.Equal(6, firstProduct.Id);
            Assert.Equal("Prod6", firstProduct.Name);
            Assert.Equal("http://image6", firstProduct.ImageUrl);
            Assert.Equal(60m, firstProduct.Price);
            Assert.Equal("Description 6", firstProduct.Description);
            Assert.Equal(12, firstProduct.StockQuantity);

            var lastProduct = actualPaged.Items.Last();
            Assert.Equal(10, lastProduct.Id);
            Assert.Equal("Prod10", lastProduct.Name);
            Assert.Equal("http://image10", lastProduct.ImageUrl);
            Assert.Equal(100m, lastProduct.Price);
            Assert.Equal("Description 10", lastProduct.Description);
            Assert.Equal(20, lastProduct.StockQuantity);
        }

        [Fact]
        public async Task GetPaged_ShouldReturnStatus500_WhenServiceThrowsException()
        {
            // Arrange
            A.CallTo(() => _productServiceFake.GetPagedProductsAsync(A<int>._, A<int>._))
                .ThrowsAsync(new Exception("Some error occurred during pagination"));

            // Act
            var result = await _controller.GetPaged(pageNumber: 1, pageSize: 10);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }
    }
}
