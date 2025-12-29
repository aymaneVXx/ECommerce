using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Services.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _auth;

    public AuthController(IAuthenticationService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUser model)
    {
        var result = await _auth.CreateUserAsync(model);
        if (!result.Success) return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUser model)
    {
        var result = await _auth.LoginUserAsync(model);
        if (!result.Success) return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var result = await _auth.ReviveTokenAsync(refreshToken);
        if (!result.Success) return BadRequest(result);

        return Ok(result);
    }
}
