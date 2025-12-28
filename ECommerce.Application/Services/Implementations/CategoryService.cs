using AutoMapper;
using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<GetCategoryDto>> GetAllAsync()
    {
        var categories = await _repo.GetAllAsync();
        return _mapper.Map<List<GetCategoryDto>>(categories);
    }

    public async Task<GetCategoryDto?> GetByIdAsync(Guid id)
    {
        var category = await _repo.GetByIdAsync(id);
        return category is null ? null : _mapper.Map<GetCategoryDto>(category);
    }

    public async Task<ServiceResponse<GetCategoryDto>> AddAsync(CreateCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return ServiceResponse<GetCategoryDto>.Fail("Category name is required.");

        var entity = _mapper.Map<Category>(dto);
        var created = await _repo.AddAsync(entity);

        return ServiceResponse<GetCategoryDto>.Ok(_mapper.Map<GetCategoryDto>(created), "Category added.");
    }

    public async Task<ServiceResponse<GetCategoryDto>> UpdateAsync(UpdateCategoryDto dto)
    {
        if (dto.Id == Guid.Empty)
            return ServiceResponse<GetCategoryDto>.Fail("Category id is required.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            return ServiceResponse<GetCategoryDto>.Fail("Category name is required.");

        var entity = _mapper.Map<Category>(dto);
        var updated = await _repo.UpdateAsync(entity);

        if (updated is null)
            return ServiceResponse<GetCategoryDto>.Fail("Category not found.");

        return ServiceResponse<GetCategoryDto>.Ok(_mapper.Map<GetCategoryDto>(updated), "Category updated.");
    }

    public async Task<ServiceResponse> DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            return ServiceResponse.Fail("Category id is required.");

        var deleted = await _repo.DeleteAsync(id);
        return deleted ? ServiceResponse.Ok("Category deleted.") : ServiceResponse.Fail("Category not found.");
    }
}
