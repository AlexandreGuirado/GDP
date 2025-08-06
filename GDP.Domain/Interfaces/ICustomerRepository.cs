using GDP.Domain.Entities;
using System.Linq.Expressions;

namespace GDP.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<IEnumerable<Customer>> SearchAsync(string? name, string? email);
    Task<int> GetTotalCountAsync();
    Task<IEnumerable<Customer>> ExistsExactAsync(string? name, string? email, string? phone);
}