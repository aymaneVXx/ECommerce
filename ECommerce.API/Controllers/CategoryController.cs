using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoryController(ICategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<GetCategoryDto>>> GetAll()
    {
        var categories = await _service.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetCategoryDto>> GetById(Guid id)
    {
        var category = await _service.GetByIdAsync(id);
        if (category is null) return NotFound("Category not found.");
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult> Add([FromBody] CreateCategoryDto dto)
    {
        var result = await _service.AddAsync(dto);
        if (!result.Success) return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult> Update([FromBody] UpdateCategoryDto dto)
    {
        var result = await _service.UpdateAsync(dto);
        if (!result.Success)
        {
            if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(result.Message);

            return BadRequest(result.Message);
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result.Success)
        {
            if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(result.Message);

            return BadRequest(result.Message);
        }

        return Ok(result);
    }
}
