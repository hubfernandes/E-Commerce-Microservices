using AutoMapper;
using InventoryService.Domain.Dtos;
using MediatR;
using ProductService.Application.Queries;
using ProductService.Domain.Dtos;
using ProductService.Infrastructure.Interfaces;
using Shared.Bases;
using System.Text.Json;

namespace ProductService.Application.Handlers
{
    public class ProductQueryHandler : IRequestHandler<GetProductByIdQuery, Response<ProductDto>>,
                                       IRequestHandler<GetAllProductsQuery, Response<List<ProductDto>>>,
                                       IRequestHandler<GetProductStockQuery, Response<int>>,
                                       IRequestHandler<GetLowStockProductsQuery, Response<List<ProductStockDto>>>,
                                       IRequestHandler<SearchProductsQuery, Response<List<ProductDto>>>

    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public readonly ResponseHandler _responseHandler;
        private readonly HttpClient _inventoryClient;

        public ProductQueryHandler(ResponseHandler responseHandler, IProductRepository productRepository, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _responseHandler = responseHandler;
            _inventoryClient = httpClientFactory.CreateClient("InventoryService");

        }

        public async Task<Response<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                return _responseHandler.NotFound<ProductDto>("Product not Found");
            }
            var mappedProduct = _mapper.Map<ProductDto>(product);
            return _responseHandler.Success(mappedProduct);
        }

        public async Task<Response<List<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllAsync();
            var mappedProduct = _mapper.Map<List<ProductDto>>(products);
            return _responseHandler.Success(mappedProduct);
        }

        public async Task<Response<List<ProductDto>>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetByAsync(p => p.Name.Contains(request.Query));
            var mappedProducts = _mapper.Map<List<ProductDto>>(products);
            return _responseHandler.Success(mappedProducts);
        }

        public async Task<Response<int>> Handle(GetProductStockQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _inventoryClient.GetAsync($"api/Inventory/{request.ProductId}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    return _responseHandler.NotFound<int>("Inventory not found for product");
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var inventoryItem = JsonSerializer.Deserialize<Response<StockDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return _responseHandler.Success(inventoryItem!.Data!.QuantityAvailable);
            }
            catch (HttpRequestException ex)
            {
                return _responseHandler.BadRequest<int>($"Failed to fetch stock: {ex.Message}");
            }
        }

        public async Task<Response<List<ProductStockDto>>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _inventoryClient.GetAsync($"api/Inventory/low-stock?threshold={request.Threshold}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    return _responseHandler.NotFound<List<ProductStockDto>>("No low stock items found");
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var lowStockItems = JsonSerializer.Deserialize<Response<List<LowStockDto>>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (!lowStockItems!.Data!.Any())
                {
                    return _responseHandler.Success(new List<ProductStockDto>());
                }

                var productIds = lowStockItems!.Data!.Select(i => i.ProductId).ToList();
                var productStockDtos = await _productRepository.GetLowStockProductsAsync(productIds, lowStockItems!.Data!);
                return _responseHandler.Success(productStockDtos);
            }
            catch (HttpRequestException ex)
            {
                return _responseHandler.BadRequest<List<ProductStockDto>>($"Failed to fetch low stock items: {ex.Message}");
            }
        }
    }
}
