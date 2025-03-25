using AutoMapper;
using CartService.Application.Commands;
using CartService.Application.Validators;
using CartService.Domain.Entities;
using CartService.Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Bases;
using System.Security.Claims;

namespace CartService.Application.Handlers
{
    public class CartCommandHandler : IRequestHandler<CreateCartCommand, Response<string>>,
                                      IRequestHandler<UpdateCartCommand, Response<string>>,
                                      IRequestHandler<DeleteCartCommand, Response<string>>,
                                      IRequestHandler<DeleteCartByUserIdCommand, Response<string>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly IValidateCartExists _validateCartExists;
        public readonly ResponseHandler _responseHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartCommandHandler(
            ICartRepository cartRepository,
            ResponseHandler responseHandler,
            IMapper mapper,
            IValidateCartExists validateCartExists,
            IHttpContextAccessor httpContextAccessor)
        {
            _cartRepository = cartRepository;
            _validateCartExists = validateCartExists;
            _mapper = mapper;
            _responseHandler = responseHandler;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<string>> Handle(CreateCartCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new UnauthorizedAccessException("User ID not found in token.");

                if (request.UserId != userId)
                    return _responseHandler.BadRequest<string>("User ID in request does not match authenticated user.");

                var cart = _mapper.Map<Cart>(request);
                var addedCart = await _cartRepository.AddAsync(cart);
                return _responseHandler.Created<string>("Cart Created Successfully");
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



        public async Task<Response<string>> Handle(UpdateCartCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _validateCartExists.ValidateCartExistsAsync(request.Id);
                var cart = _mapper.Map<Cart>(request);
                var updatedCart = await _cartRepository.UpdateAsync(cart);
                return _responseHandler.Success<string>("Cart Updated Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return _responseHandler.NotFound<string>(ex.Message);
            }
        }

        public async Task<Response<string>> Handle(DeleteCartCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _validateCartExists.ValidateCartExistsAsync(request.Id);
                await _cartRepository.DeleteByIdAsync(request.Id);
                return _responseHandler.Success<string>("Cart Deleted Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return _responseHandler.NotFound<string>(ex.Message);
            }
        }

        public async Task<Response<string>> Handle(DeleteCartByUserIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new UnauthorizedAccessException("User ID not found in token.");

                if (request.UserId != userId)
                    return _responseHandler.BadRequest<string>("User ID in request does not match authenticated user.");

                var cart = await _cartRepository.GetByUserIdAsync(request.UserId);
                // var cart = await _cartRepository.GetByUserIdAsync("e7b99a97-553b-49a6-8b0d-f28cae2691f5");
                if (cart == null)
                    return _responseHandler.NotFound<string>("Cart not found for the specified user.");

                await _cartRepository.DeleteByUserIdAsync(request.UserId);
                //await _cartRepository.DeleteByUserIdAsync("e7b99a97-553b-49a6-8b0d-f28cae2691f5");
                return _responseHandler.Success<string>("Cart deleted successfully");
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
    }
}