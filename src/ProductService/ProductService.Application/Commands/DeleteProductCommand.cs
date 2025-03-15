using MediatR;
using Shared.Bases;
namespace ProductService.Application.Commands
{
    public record DeleteProductCommand(int Id) : IRequest<Response<string>>;

}
