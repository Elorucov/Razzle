using ELOR.Razzle.API;
using ELOR.Razzle.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ELOR.Razzle.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AuthRequiredAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.RequestServices.GetRequiredService<UserSession>();
            if (!user.IsAuthenticated)
            {
                context.Result = new ObjectResult(new ErrorEnvelope
                {
                    Error = new ApiError
                    {
                        Code = ErrorCodes.AuthRequired,
                        Message = "Authorization failed: access token not passed or invalid"
                    }
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }

            return Task.CompletedTask;
        }
    }
}
