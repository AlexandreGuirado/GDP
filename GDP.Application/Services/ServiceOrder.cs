using GDP.Application.DTOs;
using GDP.Domain.Entities;
using GDP.Domain.Enums;
using GDP.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GDP.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<(bool Success, string ErrorMessage, int? CreatedOrderId)> CreateOrderAsync(CreateOrderDto orderDto)
    {
        if (orderDto.Items == null || !orderDto.Items.Any())
        {
            return (false, "An order must contain at least one item.", null);
        }

        var totalAmount = 0m;
        var orderItems = new List<Domain.Entities.OrderItem>();

        foreach (var itemDto in orderDto.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
            if (product == null)
            {
                return (false, $"Product with ID {itemDto.ProductId} not found.", null);
            }
            if (product.Stock < itemDto.Quantity)
            {
                return (false, $"Insufficient stock for product '{product.Name}'. Available: {product.Stock}, Requested: {itemDto.Quantity}.", null);
            }

            orderItems.Add(new Domain.Entities.OrderItem
            {
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price
            });
            totalAmount += product.Price * itemDto.Quantity;
        }

        var order = new Order
        {
            CustomerId = orderDto.CustomerId,
            OrderDate = DateTime.Now,
            Status = OrderStatus.Pending.ToString(),
            TotalAmount = totalAmount,
            Items = orderItems
        };

        try
        {
            var newOrderId = await _orderRepository.AddAsync(order);
            _logger.LogInformation("Order {OrderId} created successfully for customer {CustomerId}.", newOrderId, orderDto.CustomerId);
            _logger.LogInformation("NOTIFICATION: Status for Order {OrderId} changed to {Status}", newOrderId, order.Status);
            return (true, "Order created successfully!", newOrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order for customer {CustomerId}", orderDto.CustomerId);
            return (false, "An unexpected error occurred while creating the order. Please try again.", null);
        }
    }
}