using ELOR.Razzle.API;
using ELOR.Razzle.Services;

namespace ELOR.Razzle.Middlewares
{
    public sealed class APIExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<APIExceptionMiddleware> _logger;

        public APIExceptionMiddleware(RequestDelegate next, ILogger<APIExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ServiceException ex)
            {
                await ReturnAPIErrorAsync(context, ex);
            }
            catch (AggregateException ex) when (ex.InnerException != null && ex.InnerException is ServiceException appex) 
            {
                await ReturnAPIErrorAsync(context, appex);
            }
            catch (AggregateException ex) when (ex.InnerException != null && ex.InnerException is not ServiceException)
            {
                _logger.LogError(ex, "Unhandled error (aggregate)");
                await ReturnInternalErrorAsync(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error");
                await ReturnInternalErrorAsync(context);
            }
        }

        private async Task ReturnAPIErrorAsync(HttpContext context, ServiceException ex)
        {
            await APIResults.WriteError(context,
                    new ApiError { Code = ex.Code, Message = ex.Message, RequestParams = ex.RequestParams },
                    ex.StatusCode, context.RequestAborted);
        }

        private async Task ReturnInternalErrorAsync(HttpContext context)
        {
            await APIResults.WriteError(context,
                    new ApiError { Code = ErrorCodes.InternalError, Message = "Internal server error" },
                    StatusCodes.Status500InternalServerError, context.RequestAborted);
        }
    }
}
