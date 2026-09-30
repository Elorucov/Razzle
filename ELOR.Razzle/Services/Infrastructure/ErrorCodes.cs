namespace ELOR.Razzle.Services.Infrastructure
{
    public static class ErrorCodes
    {
        public const ushort InternalError = 1;
        public const ushort UnknownMethod = 3;
        public const ushort AuthRequired = 5;
        public const ushort TooManyRequests = 6;
        public const ushort InvalidParameter = 100;

        public const ushort InvalidLoginOrPassword = 101;
        public const ushort UserAlreadyExists = 103;
        public const ushort NotFound = 104;
        public const ushort AlreadyExists = 105;

        public const ushort NoTagsFound = 200;

        public const ushort NoTaskFound = 300;
    }
}
