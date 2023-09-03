using Grpc.Core;
using Discount.API;
using Discount.Application.Commands;
using Discount.Application.Queries;
using Discount.Grpc.Protos;
using MediatR;

namespace Discount.API.Services;

public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
{
    private readonly ILogger<DiscountService> _logger;
    private readonly IMediator _mediator;

    public DiscountService(ILogger<DiscountService> logger,IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var query = new GetDiscountQuery(request.ProductName);
        var result = await _mediator.Send(query);
        _logger.LogInformation($"Discount is retrieved for the Product Name:{result.ProductName} and Amount:{result.Amount}");
        return result;
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var cmd = new CreateDiscountCommand
        {
            ProductName = request.Coupon.ProductName,
            Amount = request.Coupon.Amount,
            Description = request.Coupon.Description
        };
        var result = await _mediator.Send(cmd);
        _logger.LogInformation($"Discount is successfully created for the Product Name: {result.ProductName}");
        return result;
    }
    
    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var cmd = new UpdateDiscountCommand
        {
            Id = request.Coupon.Id,
            ProductName = request.Coupon.ProductName,
            Amount = request.Coupon.Amount,
            Description = request.Coupon.Description
        };
        var result = await _mediator.Send(cmd);
        _logger.LogInformation($"Discount is successfully updated Product Name: {result.ProductName}");
        return result;
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        var cmd = new DeleteDiscountCommand(request.ProductName);
        var deleted = await _mediator.Send(cmd);
        var response = new DeleteDiscountResponse
        {
            Success = deleted
        };
        return response;
    }
}