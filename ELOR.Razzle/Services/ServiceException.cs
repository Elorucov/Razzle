using System.Text;

namespace ELOR.Razzle.Services
{
    public sealed class ServiceException : ApplicationException
    {
        public ServiceException(ushort code, string message, int statusCode, Dictionary<string, string> requestParams = null)
        : base(message)
        {
            Code = code;
            Message = message;
            StatusCode = statusCode;
            RequestParams = requestParams?.AsReadOnly();
        }

        public ushort Code { get; }
        public override string Message { get; }
        public int StatusCode { get; }
        public IReadOnlyDictionary<string, string> RequestParams { get; }

        public static ServiceException InvalidParam(string param, string reason) =>
            new(ErrorCodes.InvalidParameter, "One of the parameters specified was missing or invalid", StatusCodes.Status400BadRequest,
                new Dictionary<string, string>() { { param, reason } });

        public static ServiceException AuthFailed() =>
            new(ErrorCodes.InvalidLoginOrPassword, "Invalid login or password", StatusCodes.Status401Unauthorized);

        public static ServiceException UserAlreadyExists() =>
            new(ErrorCodes.UserAlreadyExists, "User with this username already exists", StatusCodes.Status409Conflict);

        public static ServiceException NotFound() =>
            new(ErrorCodes.NotFound, "Not found", StatusCodes.Status404NotFound);

        public static ServiceException AlreadyExists() =>
            new(ErrorCodes.AlreadyExists, "Already exists", StatusCodes.Status400BadRequest);
    }
}