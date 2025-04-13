using AutoMapper;
using ProductManagmentSystem.Application.Interfaces;
using ProductManagmentSystem.Contracts.DTOs;
using ProductManagmentSystem.Contracts.DTOs.Product;
using ProductManagmentSystem.Domain.Entities;
using ProductManagmentSystem.Domain.RepositoryInterfaces;

namespace ProductManagmentSystem.Application.Services
{
    public class ProductService: IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository, IMapper mapper)
        {
            _productRepository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id) =>
            _mapper.Map<ProductDto?>(await _productRepository.GetByIdAsync(id));

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);

            await _productRepository.AddAsync(product);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task UpdateProductStockAsync(int id, int stockQuantity)
        {
            var product = await _productRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Product with ID {id} not found.");
            product.StockQuantity = stockQuantity;
            await _productRepository.UpdateProductStockAsync(product);
        }
        public async Task<PagedResult<ProductDto>> GetPagedProductsAsync(int pageNumber, int pageSize)
        {
            var (items, totalCount) = await _productRepository.GetPagedProductsAsync(pageNumber, pageSize);
            var dtos = _mapper.Map<IEnumerable<ProductDto>>(items);
            return new PagedResult<ProductDto>(dtos, totalCount, pageNumber, pageSize);
        }
    }
}
