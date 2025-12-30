using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _service;

    public CartController(ICartService service)
    {
        _service = service;
    }
    private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

    [HttpPost("checkout")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutDto dto)
    {
        var userId = CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var result = await _service.CheckoutAsync(userId, dto);
        if (!result.Success) return BadRequest(result);

        return Ok(result);
    }



    [HttpPost("save-checkout")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SaveCheckout([FromBody] List<CreateCheckoutArchiveDto> archives)
    {
        var userId = CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var result = await _service.SaveCheckoutHistoryAsync(userId, archives);
        if (!result.Success) return BadRequest(result.Message);

        return Ok(result);
    }


    [HttpGet("archives")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<GetCheckoutArchiveDto>>> Archives()
    {
        var archives = await _service.GetCheckoutHistoryAsync();
        if (archives is null || archives.Count == 0) return NotFound("No archives found.");
        return Ok(archives);
    }
}
