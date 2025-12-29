using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Responses;

namespace ECommerce.Application.Services.Authentication;

public interface IAuthenticationService
{
    Task<ServiceResponse> CreateUserAsync(CreateUser model);
    Task<LoginResponse> LoginUserAsync(LoginUser model);
    Task<LoginResponse> ReviveTokenAsync(string refreshToken);
}

