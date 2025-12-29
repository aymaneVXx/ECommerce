using ECommerce.Application.Mapping;
using ECommerce.Application.Services;
using ECommerce.Application.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;
using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Services.Authentication;
using ECommerce.Application.Validations.Authentication;
using FluentValidation;

namespace ECommerce.Application.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IValidator<CreateUser>, CreateUserValidator>();
        services.AddScoped<IValidator<LoginUser>, LoginUserValidator>();

        return services;
    }
}
