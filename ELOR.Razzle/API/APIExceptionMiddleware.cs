using ELOR.Razzle.Services;

namespace ELOR.Razzle.API
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
                await APIResults.WriteError(context,
                    new ApiError { Code = ex.Code, Message = ex.Message, RequestParams = ex.RequestParams },
                    ex.StatusCode, context.RequestAborted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error");
                await APIResults.WriteError(context,
                    new ApiError { Code = ErrorCodes.InternalError, Message = "Internal server error" },
                    StatusCodes.Status500InternalServerError, context.RequestAborted);
            }
        }
    }
}
