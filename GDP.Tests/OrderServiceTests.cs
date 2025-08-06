using Moq;
using Microsoft.Extensions.Logging;
using GDP.Domain.Interfaces;
using GDP.Domain.Entities;
using GDP.Application.DTOs;
using GDP.Application.Services;

namespace GDP.Tests;

[TestClass]
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepo;
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<ILogger<OrderService>> _mockLogger;
    private readonly IOrderService _orderService;

    public OrderServiceTests()
    {
        _mockOrderRepo = new Mock<IOrderRepository>();
        _mockProductRepo = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<OrderService>>();

        _orderService = new OrderService(_mockOrderRepo.Object, _mockProductRepo.Object, _mockLogger.Object);
    }

    [TestMethod]
    public async Task CreateOrderAsync_ShouldFail_WhenStockIsInsufficient()
    {
        var productWithLowStock = new Product { Id = 1, Name = "Test Product", Stock = 5, Price = 10 };
        var orderDto = new CreateOrderDto
        {
            CustomerId = 1,
            Items = new List<OrderItemDto> { new OrderItemDto { ProductId = 1, Quantity = 10 } }
        };

        _mockProductRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(productWithLowStock);

        var (success, errorMessage, _) = await _orderService.CreateOrderAsync(orderDto);

        Assert.IsFalse(success, "A criação do pedido deveria ter falhado.");
        Assert.IsTrue(errorMessage.Contains("Insufficient stock"), "A mensagem de erro deveria mencionar estoque insuficiente.");

        _mockOrderRepo.Verify(repo => repo.AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [TestMethod]
    public async Task CreateOrderAsync_ShouldSucceed_WhenStockIsSufficient()
    {
        var productWithStock = new Product { Id = 1, Name = "Test Product", Stock = 20, Price = 10 };
        var orderDto = new CreateOrderDto
        {
            CustomerId = 1,
            Items = new List<OrderItemDto> { new OrderItemDto { ProductId = 1, Quantity = 10 } }
        };

        _mockProductRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(productWithStock);

        _mockOrderRepo.Setup(repo => repo.AddAsync(It.IsAny<Order>())).ReturnsAsync(123);

        var (success, errorMessage, createdOrderId) = await _orderService.CreateOrderAsync(orderDto);

        Assert.IsTrue(success, "A criação do pedido deveria ter sido bem-sucedida.");
        Assert.AreEqual(123, createdOrderId, "O ID do pedido criado não é o esperado.");

        _mockOrderRepo.Verify(repo => repo.AddAsync(It.IsAny<Order>()), Times.Once);
    }
}