using ELOR.Razzle.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ELOR.Razzle.API
{
    // Runs model-binding checks and FluentValidation, producing VK-style invalid-parameter errors.
    public sealed class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _services;

        public ValidationFilter(IServiceProvider services)
        {
            _services = services;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var bindingErrors = context.ModelState
                    .Where(entry => entry.Value is { Errors.Count: > 0 })
                    .Select(entry =>
                    {
                        var message = entry.Value!.Errors[0].ErrorMessage;
                        return new Tuple<string, string>(ToCamelCase(entry.Key),
                            string.IsNullOrEmpty(message) ? "Invalid value" : message);
                    })
                    .ToDictionary(t => t.Item1, t => t.Item2);

                context.Result = ErrorResult(bindingErrors);
                return;
            }

            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument is null)
                {
                    continue;
                }

                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
                if (_services.GetService(validatorType) is not IValidator validator)
                {
                    continue;
                }

                var result = await validator.ValidateAsync(new ValidationContext<object>(argument), context.HttpContext.RequestAborted);
                if (!result.IsValid)
                {
                    var errors = result.Errors
                        .ToDictionary(failure => failure.PropertyName, failure => failure.ErrorMessage);

                    context.Result = ErrorResult(errors);
                    return;
                }
            }

            await next();
        }

        private static ObjectResult ErrorResult(Dictionary<string, string> errors) =>
            new(new ErrorEnvelope
            {
                Error = new ApiError
                {
                    Code = ErrorCodes.InvalidParameter,
                    Message = "One of the parameters specified was missing or invalid",
                    RequestParams = errors
                }
            })
            {
                StatusCode = StatusCodes.Status400BadRequest
            };

        private static string ToCamelCase(string value) =>
            string.IsNullOrEmpty(value) ? value : char.ToLowerInvariant(value[0]) + value[1..];
    }
}
