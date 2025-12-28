using ECommerce.Application.Mapping;
using ECommerce.Application.Services;
using ECommerce.Application.Services.Implementations;
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

        return services;
    }
}
