
using GDP.Application.DTOs;

namespace GDP.Domain.Interfaces;

public interface IOrderService
{
    Task<(bool Success, string ErrorMessage, int? CreatedOrderId)> CreateOrderAsync(CreateOrderDto orderDto);
}