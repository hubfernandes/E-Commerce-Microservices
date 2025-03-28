using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Bases;
using System.Security.Claims;
using WishlistService.Application.Commands;
using WishlistService.Application.Validators;
using WishlistService.Domain.Entities;
using WishlistService.Infrastructure.Interfaces;

namespace WishlistService.Application.Handlers
{
    public class WishlistCommandHandler :
         IRequestHandler<CreateWishlistCommand, Response<string>>,
         IRequestHandler<UpdateWishlistCommand, Response<string>>,
         IRequestHandler<DeleteWishlistCommand, Response<string>>,
         IRequestHandler<DeleteWishlistByUserIdCommand, Response<string>>,
         IRequestHandler<AddItemToWishlistCommand, Response<string>>,
         IRequestHandler<RemoveItemFromWishlistCommand, Response<string>>
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IMapper _mapper;
        private readonly IValidateWishlistExists _validateWishlistExists;
        private readonly ResponseHandler _responseHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WishlistCommandHandler(
            IWishlistRepository wishlistRepository,
            IMapper mapper,
            IValidateWishlistExists validateWishlistExists,
            ResponseHandler responseHandler,
            IHttpContextAccessor httpContextAccessor)
        {
            _wishlistRepository = wishlistRepository;
            _mapper = mapper;
            _validateWishlistExists = validateWishlistExists;
            _responseHandler = responseHandler;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<string>> Handle(CreateWishlistCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new UnauthorizedAccessException("User ID not found in token.");
                if (request.UserId != userId)
                    return _responseHandler.BadRequest<string>("User ID mismatch.");

                var wishlist = _mapper.Map<Wishlist>(request);
                await _wishlistRepository.AddAsync(wishlist);
                return _responseHandler.Created<string>("Wishlist created successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return _responseHandler.Unauthorized<string>(ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<string>(ex.Message);
            }
        }

        public async Task<Response<string>> Handle(UpdateWishlistCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _validateWishlistExists.ValidateWishlistExistsAsync(request.Id);
                var wishlist = _mapper.Map<Wishlist>(request);
                await _wishlistRepository.UpdateAsync(wishlist);
                return _responseHandler.Success<string>("Wishlist updated successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return _responseHandler.NotFound<string>(ex.Message);
            }
        }

        public async Task<Response<string>> Handle(DeleteWishlistCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _validateWishlistExists.ValidateWishlistExistsAsync(request.Id);
                await _wishlistRepository.DeleteByIdAsync(request.Id);
                return _responseHandler.Success<string>("Wishlist deleted successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return _responseHandler.NotFound<string>(ex.Message);
            }
        }

        public async Task<Response<string>> Handle(DeleteWishlistByUserIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new UnauthorizedAccessException("User ID not found in token.");
                if (request.UserId != userId)
                    return _responseHandler.BadRequest<string>("User ID mismatch.");

                var wishlist = await _wishlistRepository.GetByUserIdAsync(request.UserId);
                if (wishlist == null)
                    return _responseHandler.NotFound<string>("Wishlist not found for user.");

                await _wishlistRepository.DeleteByUserIdAsync(request.UserId);
                return _responseHandler.Success<string>("Wishlist deleted successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return _responseHandler.Unauthorized<string>(ex.Message);
            }
        }

        public async Task<Response<string>> Handle(AddItemToWishlistCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new UnauthorizedAccessException("User ID not found in token.");
                if (request.UserId != userId)
                    return _responseHandler.BadRequest<string>("User ID mismatch.");

                var wishlist = await _wishlistRepository.GetByUserIdAsync(request.UserId);
                if (wishlist == null)
                {
                    wishlist = new Wishlist { UserId = request.UserId };
                    await _wishlistRepository.AddAsync(wishlist);
                }

                await _wishlistRepository.AddItemAsync(request.UserId, request.ProductId);
                return _responseHandler.Success<string>("Item added to wishlist");
            }
            catch (UnauthorizedAccessException ex)
            {
                return _responseHandler.Unauthorized<string>(ex.Message);
            }
        }

        public async Task<Response<string>> Handle(RemoveItemFromWishlistCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new UnauthorizedAccessException("User ID not found in token.");
                if (request.UserId != userId)
                    return _responseHandler.BadRequest<string>("User ID mismatch.");

                var wishlist = await _wishlistRepository.GetByUserIdAsync(request.UserId);
                if (wishlist == null)
                    return _responseHandler.NotFound<string>("Wishlist not found for user.");

                await _wishlistRepository.RemoveItemAsync(request.UserId, request.ProductId);
                return _responseHandler.Success<string>("Item removed from wishlist");
            }
            catch (UnauthorizedAccessException ex)
            {
                return _responseHandler.Unauthorized<string>(ex.Message);
            }
        }
    }
}
