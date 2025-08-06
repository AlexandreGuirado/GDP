using Dapper;
using GDP.Domain.Entities;
using GDP.Domain.Interfaces;
using GDP.Infrastructure.Data;
using System.Data;

namespace GDP.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public ProductRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> AddAsync(Product entity)
    {
        const string sql = "INSERT INTO Products (Name, Description, Price, Stock) VALUES (@Name, @Description, @Price, @Stock); SELECT SCOPE_IDENTITY();";
        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Products WHERE Id = @Id";
        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.ExecuteAsync(sql, new { Id = id }) > 0;
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string? name)
    {
        var sql = "SELECT * FROM Products WHERE 1=1 ";
        if (!string.IsNullOrWhiteSpace(name))
        {
            sql += " AND Name LIKE @Name";
        }
        sql += " ORDER BY Name";

        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.QueryAsync<Product>(sql, new { Name = $"%{name}%" });
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        const string sql = "SELECT * FROM Products ORDER BY Name";
        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.QueryAsync<Product>(sql);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM Products WHERE Id = @Id";
        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.QuerySingleOrDefaultAsync<Product>(sql, new { Id = id });
    }

    public async Task<bool> UpdateAsync(Product entity)
    {
        const string sql = "UPDATE Products SET Name = @Name, Description = @Description, Price = @Price, Stock = @Stock WHERE Id = @Id";
        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.ExecuteAsync(sql, entity) > 0;
    }

    public async Task<int> GetTotalCountAsync()
    {
        const string sql = "SELECT COUNT(Id) FROM Products";
        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.ExecuteScalarAsync<int>(sql);
    }
}