namespace ELOR.Razzle.Services
{
    public static class ErrorCodes
    {
        public const int InternalError = 1;
        public const int UnknownMethod = 3;
        public const int AuthRequired = 5;
        public const int TooManyRequests = 6;
        public const int InvalidParameter = 100;

        public const int InvalidLoginOrPassword = 101;
        public const int UserAlreadyExists = 103;
        public const int NotFound = 104;
        public const int AlreadyExists = 105;
    }
}
