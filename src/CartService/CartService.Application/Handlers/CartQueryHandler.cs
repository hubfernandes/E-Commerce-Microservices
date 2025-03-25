using AutoMapper;
using CartService.Application.Queries;
using CartService.Domain.Dtos;
using CartService.Infrastructure.Interfaces;
using MediatR;
using Shared.Bases;

namespace CartService.Application.Handlers
{
    public class CartQueryHandler :
        IRequestHandler<GetCartByIdQuery, Response<CartDto>>,
        IRequestHandler<GetAllCartsQuery, Response<List<CartDto>>>,
        IRequestHandler<GetCartsByUserIdQuery, Response<List<CartItemDto>>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        public readonly ResponseHandler _responseHandler;

        public CartQueryHandler(
            ResponseHandler responseHandler,
            ICartRepository cartRepository,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _responseHandler = responseHandler;
        }

        public async Task<Response<List<CartItemDto>>> Handle(GetCartsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var carts = await _cartRepository.GetCartItemsByUserIdAsync(request.UserId);
            if (carts == null || carts.Count == 0)
            {
                return _responseHandler.NotFound<List<CartItemDto>>("No carts found for this user.");
            }
            var mappedCarts = _mapper.Map<List<CartItemDto>>(carts);
            return _responseHandler.Success(mappedCarts);
        }


        public async Task<Response<CartDto>> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(request.Id);
            if (cart == null)
            {
                return _responseHandler.NotFound<CartDto>("Cart not Found");
            }
            var mappedCart = _mapper.Map<CartDto>(cart);
            return _responseHandler.Success(mappedCart);
        }

        public async Task<Response<List<CartDto>>> Handle(GetAllCartsQuery request, CancellationToken cancellationToken)
        {
            var carts = await _cartRepository.GetAllAsync();
            var mappedCarts = _mapper.Map<List<CartDto>>(carts);
            return _responseHandler.Success(mappedCarts);
        }
    }
}