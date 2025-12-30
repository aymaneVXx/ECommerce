using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(Guid id);
    Task<List<Product>> GetByCategoryIdAsync(Guid categoryId);
    Task<Product> AddAsync(Product product);
    Task<Product?> UpdateAsync(Product product);
    Task<bool> DeleteAsync(Guid id);

    Task<List<Product>> GetProductsByIdsAsync(IEnumerable<Guid> ids);

}
