using Catalog.Application.Responses;
using MediatR;

namespace Catalog.Application.Queries;

public class DeleteProductByIdQuery : IRequest<bool>, IRequest<ProductResponse>
{
    public string Id { get; set; }

    public DeleteProductByIdQuery(string id)
    {
        Id = id;
    }
}