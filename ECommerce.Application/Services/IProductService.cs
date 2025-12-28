using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Services;

public interface IProductService
{
    Task<List<GetProductDto>> GetAllAsync();
    Task<GetProductDto?> GetByIdAsync(Guid id);
    Task<List<GetProductDto>> GetByCategoryIdAsync(Guid categoryId);
    Task<ServiceResponse<GetProductDto>> AddAsync(CreateProductDto dto);
    Task<ServiceResponse<GetProductDto>> UpdateAsync(UpdateProductDto dto);
    Task<ServiceResponse> DeleteAsync(Guid id);
}

