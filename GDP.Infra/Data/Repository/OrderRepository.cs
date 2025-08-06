using Dapper;
using GDP.Domain.Entities;
using GDP.Domain.Enums;
using GDP.Domain.Interfaces;
using GDP.Infrastructure.Data;
using System.Data;

namespace GDP.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public OrderRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task<int> AddAsync(Order entity)
    {
        using IDbConnection db = _connectionFactory.CreateConnection();
        db.Open();
        using var transaction = db.BeginTransaction();
        try
        {
            const string orderSql = "INSERT INTO Orders (CustomerId, OrderDate, TotalAmount, Status) VALUES (@CustomerId, @OrderDate, @TotalAmount, @Status); SELECT SCOPE_IDENTITY();";
            var orderId = await db.ExecuteScalarAsync<int>(orderSql, entity, transaction);
            transaction.Commit();
            return orderId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<int> AddAsync(Order entity, IDbTransaction transaction)
    {
        const string orderSql = "INSERT INTO Orders (CustomerId, OrderDate, TotalAmount, Status) VALUES (@CustomerId, @OrderDate, @TotalAmount, @Status); SELECT SCOPE_IDENTITY();";

        // O Dapper usa a conexão da transação
        var orderId = await transaction.Connection.ExecuteScalarAsync<int>(orderSql, entity, transaction);
        entity.Id = orderId;

        foreach (var item in entity.Items)
        {
            item.OrderId = orderId;
            const string itemSql = "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice);";
            await transaction.Connection.ExecuteAsync(itemSql, item, transaction);
        }
        return orderId;
    }

    public async Task<Order?> GetByIdWithDetailsAsync(int id)
    {
        const string sql = @"
            SELECT * FROM Orders o WHERE o.Id = @Id;
            SELECT i.*, p.Name, p.Description 
            FROM OrderItems i 
            JOIN Products p ON i.ProductId = p.Id 
            WHERE i.OrderId = @Id;";

        using IDbConnection db = _connectionFactory.CreateConnection();
        using var multi = await db.QueryMultipleAsync(sql, new { Id = id });

        var order = await multi.ReadSingleOrDefaultAsync<Order>();
        if (order != null)
        {
            order.Items = multi.Read<OrderItem, Product, OrderItem>((item, product) =>
            {
                item.Product = product;
                return item;
            }, splitOn: "Name").AsList();
        }
        return order;
    }

    public async Task<IEnumerable<Order>> SearchWithDetailsAsync(int? customerId, string? status)
    {
        var sql = @"SELECT 
                        o.Id, o.CustomerId, o.OrderDate, o.TotalAmount, o.Status,
                        c.Id, c.Name, c.Email
                    FROM Orders o
                    JOIN Customers c ON o.CustomerId = c.Id
                    WHERE 1=1 ";

        var parameters = new DynamicParameters();
        if (customerId.HasValue)
        {
            sql += " AND o.CustomerId = @CustomerId";
            parameters.Add("CustomerId", customerId.Value);
        }
        if (!string.IsNullOrWhiteSpace(status))
        {
            sql += " AND o.Status = @Status";
            parameters.Add("Status", status);
        }
        sql += " ORDER BY o.OrderDate DESC";

        using IDbConnection db = _connectionFactory.CreateConnection();
        var orders = await db.QueryAsync<Order, Customer, Order>(sql, (order, customer) =>
        {
            order.Customer = customer;
            return order;
        }, parameters, splitOn: "Id");

        return orders;
    }

    public async Task<bool> UpdateStatusAsync(int orderId, string newStatus)
    {
        const string sql = "UPDATE Orders SET Status = @NewStatus WHERE Id = @OrderId";
        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.ExecuteAsync(sql, new { NewStatus = newStatus, OrderId = orderId }) > 0;
    }

    public Task<IEnumerable<Order>> GetAllAsync() => SearchWithDetailsAsync(null, null);
    public Task<Order?> GetByIdAsync(int id) => GetByIdWithDetailsAsync(id);
    public Task<bool> UpdateAsync(Order entity) => UpdateStatusAsync(entity.Id, entity.Status);
    public async Task<bool> DeleteAsync(int id)
    {
        return await UpdateStatusAsync(id, OrderStatus.Cancelled.ToString());
    }

    public async Task<int> GetTotalCountAsync()
    {
        const string sql = "SELECT COUNT(Id) FROM Orders";
        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.ExecuteScalarAsync<int>(sql);
    }
}