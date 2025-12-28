using ECommerce.Application.DTOs.Payment;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _service;

    public PaymentController(IPaymentService service)
    {
        _service = service;
    }

    [HttpGet("method")]
    public async Task<ActionResult<List<GetPaymentMethodDto>>> Method()
    {
        var methods = await _service.GetAllAsync();
        return Ok(methods);
    }
}
