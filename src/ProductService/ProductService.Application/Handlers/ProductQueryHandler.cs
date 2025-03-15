using AutoMapper;
using MediatR;
using ProductService.Application.Queries;
using ProductService.Domain.Dtos;
using ProductService.Infrastructure.Interfaces;

namespace ProductService.Application.Handlers
{
    public class ProductQueryHandler :
        IRequestHandler<GetProductByIdQuery, ProductDto>,
        IRequestHandler<GetAllProductsQuery, List<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }
    }
}