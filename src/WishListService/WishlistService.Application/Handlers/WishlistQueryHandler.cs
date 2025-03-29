using AutoMapper;
using MediatR;
using Shared.Bases;
using WishlistService.Application.Queries;
using WishlistService.Domain.Dtos;
using WishlistService.Infrastructure.Interfaces;

namespace WishlistService.Application.Handlers
{
    public class WishlistQueryHandler :
         IRequestHandler<GetWishlistByIdQuery, Response<WishlistDto>>,
         IRequestHandler<GetWishlistByUserIdQuery, Response<WishlistDto>>,
         IRequestHandler<GetAllWishlistsQuery, Response<List<WishlistDto>>>
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IMapper _mapper;
        private readonly ResponseHandler _responseHandler;

        public WishlistQueryHandler(IWishlistRepository wishlistRepository, IMapper mapper, ResponseHandler responseHandler)
        {
            _wishlistRepository = wishlistRepository;
            _mapper = mapper;
            _responseHandler = responseHandler;
        }

        public async Task<Response<WishlistDto>> Handle(GetWishlistByIdQuery request, CancellationToken cancellationToken)
        {
            var wishlist = await _wishlistRepository.GetByIdAsync(request.Id);
            if (wishlist == null)
                return _responseHandler.NotFound<WishlistDto>("Wishlist not found");

            var mappedWishlist = _mapper.Map<WishlistDto>(wishlist);
            return _responseHandler.Success(mappedWishlist);
        }

        public async Task<Response<WishlistDto>> Handle(GetWishlistByUserIdQuery request, CancellationToken cancellationToken)
        {
            var wishlist = await _wishlistRepository.GetByUserIdAsync(request.UserId);
            if (wishlist == null)
                return _responseHandler.NotFound<WishlistDto>("Wishlist not found for user");

            var mappedWishlist = _mapper.Map<WishlistDto>(wishlist);
            return _responseHandler.Success(mappedWishlist);
        }

        public async Task<Response<List<WishlistDto>>> Handle(GetAllWishlistsQuery request, CancellationToken cancellationToken)
        {
            var wishlists = await _wishlistRepository.GetAllAsync();
            var mappedWishlists = _mapper.Map<List<WishlistDto>>(wishlists);
            return _responseHandler.Success(mappedWishlists);
        }
    }
}
