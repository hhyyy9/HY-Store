using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Commands;
using Ordering.Application.Responses;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;

namespace Ordering.Application.Handlers;

public class CheckoutOrderHandler : IRequestHandler<CheckoutOrderCommand,int>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CheckoutOrderHandler> _logger;

    public CheckoutOrderHandler(IOrderRepository orderRepository, IMapper mapper,ILogger<CheckoutOrderHandler> logger)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        var orderEntity = _mapper.Map<Order>(request);
        var order = await _orderRepository.AddAsync(orderEntity);
        _logger.LogInformation($"Order {order} successfully created.");
        return order.Id;
    }
}