namespace ELOR.Razzle.API
{
    // Marker for already-shaped response envelopes so the result filter does not wrap them twice.
    public interface IApiEnvelope;

    // { "response": ... }
    public sealed class SuccessEnvelope : IApiEnvelope
    {
        public object Response { get; init; }
    }

    // { "error": { "code": ..., "message": ..., "requestParams": [...] } }
    public sealed class ErrorEnvelope : IApiEnvelope
    {
        public ApiError Error { get; init; } = new();
    }

    public sealed class ApiError
    {
        public int Code { get; init; }
        public string Message { get; init; } = string.Empty;
        public IReadOnlyDictionary<string, string> RequestParams { get; init; }
    }
}
