using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Mapping;
using ECommerce.Application.Services;
using ECommerce.Application.Services.Authentication;
using ECommerce.Application.Services.Implementations;
using ECommerce.Application.Services.Validations;
using ECommerce.Application.Validations.Authentication;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddScoped<IValidationService, ValidationService>();

        return services;
    }
}
