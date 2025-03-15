using AutoMapper;
using ProductService.Domain.Dtos;
using ProductService.Domain.Entities;

namespace ProductService.Application.Mappings
{
    internal class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
        }
    }
}
