using GDP.Domain.Entities;

namespace GDP.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> SearchByNameAsync(string? name);
    Task<int> GetTotalCountAsync();
}