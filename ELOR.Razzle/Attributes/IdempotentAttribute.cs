using ELOR.Razzle.API;
using ELOR.Razzle.DTO.Requests;
using ELOR.Razzle.Services.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ELOR.Razzle.Attributes
{
    // Makes a write action idempotent.
    // When the request contains an idempotency key, the first
    // successful result is cached and replayed for repeated calls with the same key.
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class IdempotentAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var key = context.ActionArguments.Values
                .OfType<IIdempotentRequest>()
                .Select(request => request.RandomId)
                .FirstOrDefault(value => value != 0);

            if (key == 0)
            {
                await next();
                return;
            }

            var user = context.HttpContext.RequestServices.GetRequiredService<UserSession>();
            var store = context.HttpContext.RequestServices.GetRequiredService<IdempotencyStore>();

            // TODO: for future, create an extension method for HttpContext
            // that returns an API method name
            var method = (context.ActionDescriptor as ControllerActionDescriptor)?.AttributeRouteInfo?.Template
                         ?? context.ActionDescriptor.DisplayName
                         ?? string.Empty;

            var cacheKey = $"idemp:{user.Username}:{method}:{key}";
            if (store.TryGet(cacheKey, out var cached))
            {
                context.Result = new ObjectResult(cached) { StatusCode = StatusCodes.Status200OK };
                return;
            }

            var executed = await next();
            if (executed.Result is ObjectResult result &&
                result.Value is not IApiEnvelope &&
                result.StatusCode is null or (>= 200 and < 300))
            {
                store.Set(cacheKey, result.Value);
            }
        }
    }
}
