using AutoMapper;
using Order.Domain.Dtos;
using Order.Domain.Entities;

namespace Order.Application.Mappings
{
    internal class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order.Domain.Entities.Order, OrderDto>();
            CreateMap<OrderDto, Order.Domain.Entities.Order>()
               .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<OrderItemDto, OrderItem>()
                .ConstructUsing(src => new OrderItem(src.ProductionId, src.Quantity, src.UnitPrice));
        }
    }
}
