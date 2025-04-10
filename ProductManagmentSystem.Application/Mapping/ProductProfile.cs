using ProductManagmentSystem.Contracts.DTOs.Product;
using ProductManagmentSystem.Domain.Entities;
using AutoMapper;

namespace ProductManagmentSystem.Application.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<CreateProductDto, Product>();
        }
    }
}
