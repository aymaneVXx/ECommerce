using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<GetProductDto>>> GetAll()
    {
        var products = await _service.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetProductDto>> GetById(Guid id)
    {
        var product = await _service.GetByIdAsync(id);
        if (product is null) return NotFound("Product not found.");
        return Ok(product);
    }

    [HttpGet("by-category/{categoryId:guid}")]
    public async Task<ActionResult<List<GetProductDto>>> GetByCategoryId(Guid categoryId)
    {
        var products = await _service.GetByCategoryIdAsync(categoryId);
        return Ok(products);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Add([FromBody] CreateProductDto dto)
    {
        var result = await _service.AddAsync(dto);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update([FromBody] UpdateProductDto dto)
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
    [Authorize(Roles = "Admin")]
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
