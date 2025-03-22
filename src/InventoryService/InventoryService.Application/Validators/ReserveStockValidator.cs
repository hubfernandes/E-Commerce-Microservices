using FluentValidation;
using InventoryService.Application.Commands;

namespace InventoryService.Application.Validators
{
    public class ReserveStockValidator : AbstractValidator<ReserveStockCommand>
    {
        public ReserveStockValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }
}
