using FluentValidation;
using FluentValidation.Results;

namespace TheEmployeeAPI;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseController :ControllerBase 
{
    protected async Task<ValidationResult> ValidateAsync<T>(T instance)
    {
        var validator = HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator == null)
        {
            throw new ArgumentException($"No validator found for {typeof(T).Name}");
        }
        var validationContext = new ValidationContext<T>(instance);

        var result = await validator.ValidateAsync(validationContext);
        return result;
    }
}