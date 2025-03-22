using AutoMapper;
using CartService.Application.Commands;
using CartService.Domain.Dtos;
using CartService.Domain.Entities;

namespace CartService.Application.Mappings
{
    internal class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<Cart, CartDto>();
            CreateMap<CartItem, CartItemDto>();
            CreateMap<CreateCartCommand, Cart>();
            CreateMap<UpdateCartCommand, Cart>();
            CreateMap<CartItemDto, CartItem>();
        }
    }
}
