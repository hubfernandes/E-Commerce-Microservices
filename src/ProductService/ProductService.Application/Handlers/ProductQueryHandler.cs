using AutoMapper;
using MediatR;
using ProductService.Application.Queries;
using ProductService.Domain.Dtos;
using ProductService.Infrastructure.Interfaces;
using Shared.Bases;

namespace ProductService.Application.Handlers
{
    public class ProductQueryHandler :
        IRequestHandler<GetProductByIdQuery, Response<ProductDto>>,
        IRequestHandler<GetAllProductsQuery, Response<List<ProductDto>>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public readonly ResponseHandler _responseHandler;


        public ProductQueryHandler(ResponseHandler responseHandler, IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _responseHandler = responseHandler;
        }

        public async Task<Response<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            var mappedProduct = _mapper.Map<ProductDto>(product);
            return _responseHandler.Success(mappedProduct);
        }

        public async Task<Response<List<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllAsync();
            var mappedProduct = _mapper.Map<List<ProductDto>>(products);
            return _responseHandler.Success(mappedProduct);
        }
    }
}