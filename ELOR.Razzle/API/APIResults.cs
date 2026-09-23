using System.Text.Json;
using System.Text.Json.Serialization;

namespace ELOR.Razzle.API
{
    // Helpers for writing error envelopes directly to the response(used outside the MVC pipeline:
    // exception middleware, rate-limiter rejection, unknown-method fallback).
    public static class APIResults
    {
        public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static async Task WriteError(HttpContext context, ApiError error, int statusCode, CancellationToken cancellationToken)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";

            var payload = JsonSerializer.Serialize(new ErrorEnvelope { Error = error }, Json);
            await context.Response.WriteAsync(payload, cancellationToken);
        }
    }
}