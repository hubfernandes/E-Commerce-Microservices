using AutoMapper;
using WishlistService.Application.Commands;
using WishlistService.Domain.Dtos;
using WishlistService.Domain.Entities;

namespace WishlistService.Application.Mapping
{
    public class WishlistProfile : Profile
    {
        public WishlistProfile()
        {
            CreateMap<CreateWishlistCommand, Wishlist>();
            CreateMap<UpdateWishlistCommand, Wishlist>();
            CreateMap<Wishlist, WishlistDto>();
            CreateMap<WishlistItem, WishlistItemDto>();
            CreateMap<WishlistItemDto, WishlistItem>();
        }
    }
}
