using ECommerce.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentMethodService _service;

    public PaymentController(IPaymentMethodService service)
    {
        _service = service;
    }

    [HttpGet("methods")]
    public async Task<IActionResult> GetMethods()
    {
        var methods = await _service.GetPaymentMethodsAsync();
        return methods.Any() ? Ok(methods) : NotFound("No payment methods found.");
    }
}
