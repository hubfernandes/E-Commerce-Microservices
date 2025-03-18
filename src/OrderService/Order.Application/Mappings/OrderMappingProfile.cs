using AutoMapper;
using Order.Domain.Dtos;
using Order.Domain.Entities;

namespace Order.Application.Mappings
{
    internal class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Domain.Entities.Order, OrderDto>()
               .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<OrderDto, Domain.Entities.Order>()
               .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<OrderItemDto, OrderItem>()
             .ConstructUsing(src => new OrderItem(src.ProductId, src.Quantity, src.UnitPrice));
        }
    }
}
