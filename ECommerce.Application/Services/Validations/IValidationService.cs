using ECommerce.Application.Responses;
using FluentValidation;

namespace ECommerce.Application.Services.Validations;

public interface IValidationService
{
    Task<ServiceResponse> ValidateAsync<T>(T model, IValidator<T> validator);
}
