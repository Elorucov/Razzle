using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ELOR.Razzle.API
{
    // Wraps successful action results into { "response": ... }.
    public sealed class APIResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult result && result.Value is not IApiEnvelope)
            {
                result.Value = new SuccessEnvelope { Response = result.Value };
                result.DeclaredType = typeof(SuccessEnvelope);
                result.StatusCode ??= StatusCodes.Status200OK;
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}