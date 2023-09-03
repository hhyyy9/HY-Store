using AutoMapper;
using Discount.Application.Commands;
using Discount.Core.Repositories;
using MediatR;

namespace Discount.Application.Handlers;

public class DeleteDiscountHandler : IRequestHandler<DeleteDiscountCommand,bool>
{
    private readonly IDiscountRepository _discountRepository;

    public DeleteDiscountHandler(IDiscountRepository discountRepository)
    {
        _discountRepository = discountRepository;
    }
    public async Task<bool> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _discountRepository.DeleteDiscount(request.ProductName);
        return deleted;
    }
}