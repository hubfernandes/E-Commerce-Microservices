using AutoMapper;
using Order.Domain.Dtos;

namespace Order.Application.Mappings
{
    internal class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order.Domain.Entities.Order, OrderDto>();
            CreateMap<OrderDto, Order.Domain.Entities.Order>();
        }
    }
}
