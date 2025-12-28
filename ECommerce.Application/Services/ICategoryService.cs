using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Services;

public interface ICategoryService
{
    Task<List<GetCategoryDto>> GetAllAsync();
    Task<GetCategoryDto?> GetByIdAsync(Guid id);
    Task<ServiceResponse<GetCategoryDto>> AddAsync(CreateCategoryDto dto);
    Task<ServiceResponse<GetCategoryDto>> UpdateAsync(UpdateCategoryDto dto);
    Task<ServiceResponse> DeleteAsync(Guid id);
}
