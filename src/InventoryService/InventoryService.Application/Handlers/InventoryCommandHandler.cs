using InventoryService.Application.Commands;
using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Interfaces;
using MediatR;
using Shared.Bases;

namespace InventoryService.Application.Handlers
{
    internal class InventoryCommandHandler : IRequestHandler<ReserveStockCommand, Response<string>>,
                                             IRequestHandler<ReleaseStockCommand, Response<string>>,
                                             IRequestHandler<UpdateStockCommand, Response<string>>,
                                             IRequestHandler<ReconcileStockCommand, Response<string>>,
                                             IRequestHandler<AddStockCommand, Response<string>>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ResponseHandler _responseHandler;

        public InventoryCommandHandler(IInventoryRepository inventoryRepository, ResponseHandler responseHandler)
        {
            _inventoryRepository = inventoryRepository;
            _responseHandler = responseHandler;
        }

        public async Task<Response<string>> Handle(AddStockCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingItem = await _inventoryRepository.GetByProductIdAsync(request.ProductId);
                if (existingItem != null)
                {
                    return _responseHandler.BadRequest<string>($"Inventory item for ProductId {request.ProductId} already exists.");
                }

                var newInventoryItem = new InventoryItem(request.ProductId, request.Quantity, request.LowStockThreshold);
                await _inventoryRepository.AddAsync(newInventoryItem);

                return _responseHandler.Success<string>("Stock added successfully.");
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<string>(ex.Message);
            }
        }
        public async Task<Response<string>> Handle(ReserveStockCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _inventoryRepository.GetByProductIdAsync(request.ProductId);
                if (item == null) return _responseHandler.NotFound<string>($"Inventory item with Id: {request.ProductId} not found");

                item.ReserveStock(request.Quantity);
                await _inventoryRepository.UpdateAsync(item);
                return _responseHandler.Success<string>("Stock reserved successfully");
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<string>(ex.Message);
            }
        }

        public async Task<Response<string>> Handle(ReleaseStockCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _inventoryRepository.GetByProductIdAsync(request.ProductId);
                if (item == null) return _responseHandler.NotFound<string>($"Inventory item with Id: {request.ProductId} not found");

                item.ReleaseStock(request.Quantity);
                await _inventoryRepository.UpdateAsync(item);
                return _responseHandler.Success<string>("Stock released successfully");
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<string>(ex.Message);
            }
        }

        public async Task<Response<string>> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _inventoryRepository.GetByProductIdAsync(request.ProductId);
                if (item == null) return _responseHandler.NotFound<string>($"Inventory item with Id: {request.ProductId} not found");

                item.UpdateStock(request.Quantity);
                await _inventoryRepository.UpdateAsync(item);
                return _responseHandler.Success<string>("Stock updated successfully");
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<string>(ex.Message);
            }
        }

        public async Task<Response<string>> Handle(ReconcileStockCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _inventoryRepository.GetByProductIdAsync(request.ProductId);
                if (item == null) return _responseHandler.NotFound<string>($"Inventory item with Id: {request.ProductId} not found");

                item.UpdateStock(request.Quantity); // Adjust stock for reconciliation
                await _inventoryRepository.UpdateAsync(item);
                return _responseHandler.Success<string>($"Stock reconciled successfully: {request.Reason}");
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<string>(ex.Message);
            }
        }
    }
}
