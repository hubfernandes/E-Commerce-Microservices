using AutoMapper;
using FluentValidation;
using MediatR;
using ProductService.Application.Commands;
using ProductService.Domain.Dtos;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Interfaces;

namespace ProductService.Application.Handlers
{
    public class ProductCommandHandler :
         IRequestHandler<CreateProductCommand, ProductDto>,
         IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateProductCommand> _createValidator;
        private readonly IValidator<UpdateProductCommand> _updateValidator;

        public ProductCommandHandler(
            IProductRepository productRepository,
            IMapper mapper,
            IValidator<CreateProductCommand> createValidator,
            IValidator<UpdateProductCommand> updateValidator)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product(0, request.Name, request.Price, request.Stock);
            var addedProduct = await _productRepository.AddAsync(product);
            return _mapper.Map<ProductDto>(addedProduct);
        }

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                return null;
            }

            //   product.Update(request.Name, request.Price, request.Stock);
            var updatedProduct = await _productRepository.UpdateAsync(product);
            return _mapper.Map<ProductDto>(updatedProduct);
        }
    }
}