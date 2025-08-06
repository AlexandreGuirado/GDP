using Dapper;
using GDP.Domain.Entities;
using GDP.Domain.Interfaces;
using GDP.Infrastructure.Data;
using System.Data;

namespace GDP.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public CustomerRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> AddAsync(Customer entity)
    {
        const string sql = "INSERT INTO Customers (Name, Email, Phone, RegistrationDate) VALUES (@Name, @Email, @Phone, @RegistrationDate); SELECT SCOPE_IDENTITY();";
        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Customers WHERE Id = @Id";
        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.ExecuteAsync(sql, new { Id = id }) > 0;
    }

    public async Task<IEnumerable<Customer>> SearchAsync(string? name, string? email)
    {
        var sql = "SELECT * FROM Customers WHERE 1=1 ";
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(name))
        {
            sql += " AND Name LIKE @Name";
            parameters.Add("Name", $"%{name}%");
        }
        if (!string.IsNullOrWhiteSpace(email))
        {
            sql += " AND Email LIKE @Email";
            parameters.Add("Email", $"%{email}%");
        }
        sql += " ORDER BY Name";

        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.QueryAsync<Customer>(sql, parameters);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        const string sql = "SELECT * FROM Customers ORDER BY Name";
        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.QueryAsync<Customer>(sql);
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM Customers WHERE Id = @Id";
        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.QuerySingleOrDefaultAsync<Customer>(sql, new { Id = id });
    }

    public async Task<bool> UpdateAsync(Customer entity)
    {
        const string sql = "UPDATE Customers SET Name = @Name, Email = @Email, Phone = @Phone WHERE Id = @Id";
        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.ExecuteAsync(sql, entity) > 0;
    }
    public async Task<int> GetTotalCountAsync()
    {
        const string sql = "SELECT COUNT(Id) FROM Customers";
        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.ExecuteScalarAsync<int>(sql);
    }
    public async Task<IEnumerable<Customer>> ExistsExactAsync(string? name, string? email, string? phone)
    {
        var sql = @"
        SELECT * FROM Customers 
        WHERE (@Name IS NULL OR Name = @Name)
           OR (@Email IS NULL OR Email = @Email)
           OR (@Phone IS NULL OR Phone = @Phone)";

        using IDbConnection db = _connectionFactory.CreateConnection();
        return await db.QueryAsync<Customer>(sql, new { Name = name, Email = email, Phone = phone });
    }
}