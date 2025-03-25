using AutoMapper;
using OrderService.Domain.Dtos;
using OrderService.Domain.Entities;

namespace OrderService.Application.Mappings
{
    internal class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderDto>()
               .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<OrderDto, Order>()
               .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<OrderItemDto, OrderItem>()
             .ConstructUsing(src => new OrderItem(src.ProductId, src.Quantity, src.UnitPrice));
        }
    }
}
