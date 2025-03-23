using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Order.Infrastructure.Messaging;
using ProductService.Application.Commands;
using ProductService.Application.Events;
using ProductService.Application.Validators;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Interfaces;
using Shared.Bases;

namespace ProductService.Application.Handlers
{
    public class ProductCommandHandler :
         IRequestHandler<CreateProductCommand, Response<string>>,
         IRequestHandler<UpdateProductCommand, Response<string>>,
         IRequestHandler<DeleteProductCommand, Response<string>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IValidateProductExists _validateProductExists;
        public readonly ResponseHandler _responseHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMessageBroker _messageBroker;

        public ProductCommandHandler(IProductRepository productRepository, ResponseHandler responseHandler, IMapper mapper, IValidateProductExists validateProductExists,
            IHttpContextAccessor httpContextAccessor, IMessageBroker messageBroker)
        {
            _productRepository = productRepository;
            _validateProductExists = validateProductExists;
            _mapper = mapper;
            _responseHandler = responseHandler;
            _httpContextAccessor = httpContextAccessor;
            _messageBroker = messageBroker;
        }

        public async Task<Response<string>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //var user = _httpContextAccessor.HttpContext?.User;
                //var userRole = user!.FindFirst(ClaimTypes.Role)?.Value;
                //var userId = user!.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("Customer ID not found in token.");


                var product = _mapper.Map<Product>(request);
                if (request.Id != 0) await _validateProductExists.ValidateProductExistsAsync(request.Id);
                var addedProduct = await _productRepository.AddAsync(product);

                // Publish event
                var productEvent = new ProductCreatedEvent(product.Id, product.Name);
                await _messageBroker.PublishAsync("product.created", productEvent);


                return _responseHandler.Created<string>("Product Created Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return _responseHandler.NotFound<string>(ex.Message);
            }
        }

        public async Task<Response<string>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _validateProductExists.ValidateProductExistsAsync(request.Id);
                var product = _mapper.Map<Product>(request);
                var updatedProduct = await _productRepository.UpdateAsync(product);
                return _responseHandler.Success<string>("Product Updated Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return _responseHandler.NotFound<string>(ex.Message);
            }
        }
        public async Task<Response<string>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _validateProductExists.ValidateProductExistsAsync(request.Id);
                await _productRepository.DeleteByIdAsync(request.Id);
                return _responseHandler.Success<string>("Product Deleted Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return _responseHandler.NotFound<string>(ex.Message);
            }
        }
    }
}