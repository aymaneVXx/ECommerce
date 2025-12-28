using AutoMapper;
using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<GetProductDto>> GetAllAsync()
    {
        var products = await _repo.GetAllAsync();
        return _mapper.Map<List<GetProductDto>>(products);
    }

    public async Task<GetProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _repo.GetByIdAsync(id);
        return product is null ? null : _mapper.Map<GetProductDto>(product);
    }

    public async Task<List<GetProductDto>> GetByCategoryIdAsync(Guid categoryId)
    {
        var products = await _repo.GetByCategoryIdAsync(categoryId);
        return _mapper.Map<List<GetProductDto>>(products);
    }

    public async Task<ServiceResponse<GetProductDto>> AddAsync(CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return ServiceResponse<GetProductDto>.Fail("Product name is required.");

        if (dto.Price <= 0)
            return ServiceResponse<GetProductDto>.Fail("Product price must be greater than 0.");

        if (dto.Quantity < 0)
            return ServiceResponse<GetProductDto>.Fail("Product quantity is invalid.");

        if (dto.CategoryId == Guid.Empty)
            return ServiceResponse<GetProductDto>.Fail("Category id is required.");

        var entity = _mapper.Map<Product>(dto);

        var created = await _repo.AddAsync(entity);
        var full = await _repo.GetByIdAsync(created.Id);

        return ServiceResponse<GetProductDto>.Ok(_mapper.Map<GetProductDto>(full!), "Product added.");
    }

    public async Task<ServiceResponse<GetProductDto>> UpdateAsync(UpdateProductDto dto)
    {
        if (dto.Id == Guid.Empty)
            return ServiceResponse<GetProductDto>.Fail("Product id is required.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            return ServiceResponse<GetProductDto>.Fail("Product name is required.");

        if (dto.Price <= 0)
            return ServiceResponse<GetProductDto>.Fail("Product price must be greater than 0.");

        if (dto.Quantity < 0)
            return ServiceResponse<GetProductDto>.Fail("Product quantity is invalid.");

        if (dto.CategoryId == Guid.Empty)
            return ServiceResponse<GetProductDto>.Fail("Category id is required.");

        var entity = _mapper.Map<Product>(dto);
        var updated = await _repo.UpdateAsync(entity);

        if (updated is null)
            return ServiceResponse<GetProductDto>.Fail("Product not found.");

        var full = await _repo.GetByIdAsync(updated.Id);

        return ServiceResponse<GetProductDto>.Ok(_mapper.Map<GetProductDto>(full!), "Product updated.");
    }

    public async Task<ServiceResponse> DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            return ServiceResponse.Fail("Product id is required.");

        var deleted = await _repo.DeleteAsync(id);
        return deleted ? ServiceResponse.Ok("Product deleted.") : ServiceResponse.Fail("Product not found.");
    }
}
