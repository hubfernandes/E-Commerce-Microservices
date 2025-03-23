using AutoMapper;
using InventoryService.Application.Queries;
using InventoryService.Domain.Dtos;
using InventoryService.Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Bases;

namespace InventoryService.Application.Handlers
{
    public class InventoryQueryHandler : IRequestHandler<GetStockByProductIdQuery, Response<StockDto>>,
                                          IRequestHandler<GetLowStockQuery, Response<List<LowStockDto>>>

    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ResponseHandler _responseHandler;

        public InventoryQueryHandler(
            IInventoryRepository inventoryRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ResponseHandler responseHandler)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _responseHandler = responseHandler;
        }

        public async Task<Response<StockDto>> Handle(GetStockByProductIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _inventoryRepository.GetByProductIdAsync(request.ProductId);
                if (item == null) return _responseHandler.NotFound<StockDto>("Inventory item not found");

                var stockDto = _mapper.Map<StockDto>(item);
                return _responseHandler.Success(stockDto);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<StockDto>(ex.Message);
            }
        }

        public async Task<Response<List<LowStockDto>>> Handle(GetLowStockQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var items = await _inventoryRepository.GetLowStockAsync(request.Threshold);
                if (items == null || !items.Any()) return _responseHandler.NotFound<List<LowStockDto>>("No low stock items found");

                var lowStockDtos = _mapper.Map<List<LowStockDto>>(items);
                return _responseHandler.Success(lowStockDtos);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<List<LowStockDto>>(ex.Message);
            }
        }
    }
}
