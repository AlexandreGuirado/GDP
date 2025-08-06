using GDP.Domain.Entities;
using System.Data;

namespace GDP.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> SearchWithDetailsAsync(int? customerId, string? status);
    Task<Order?> GetByIdWithDetailsAsync(int id);
    Task<bool> UpdateStatusAsync(int orderId, string newStatus);
    Task<int> GetTotalCountAsync();
    Task<int> AddAsync(Order entity, IDbTransaction transaction);
}