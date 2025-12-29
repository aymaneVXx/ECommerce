using ECommerce.Application.Responses;
using FluentValidation;

namespace ECommerce.Application.Services.Validations;

public class ValidationService : IValidationService
{
    public async Task<ServiceResponse> ValidateAsync<T>(T model, IValidator<T> validator)
    {
        var result = await validator.ValidateAsync(model);

        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
            return ServiceResponse.Fail(string.Join(";", errors));
        }

        return ServiceResponse.Ok();
    }
}
