using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost("checkout")]
    public ActionResult Checkout([FromBody] CheckoutDto dto)
    {
        return BadRequest("Checkout is not implemented yet. Stripe integration comes in the next steps.");
    }

    [HttpPost("save-checkout")]
    public async Task<ActionResult> SaveCheckout([FromBody] List<CreateCheckoutArchiveDto> archives)
    {
        var result = await _service.SaveCheckoutHistoryAsync(archives);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result);
    }

    [HttpGet("archives")]
    public async Task<ActionResult<List<GetCheckoutArchiveDto>>> Archives()
    {
        var archives = await _service.GetCheckoutHistoryAsync();
        if (archives is null || archives.Count == 0) return NotFound("No archives found.");
        return Ok(archives);
    }
}
