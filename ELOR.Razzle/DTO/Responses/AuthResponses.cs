namespace ELOR.Razzle.DTO.Responses
{
    public sealed class SignInResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        // Token expiry as unix time, or 0 if the token never expires.
        public long ExpiresIn { get; set; }
    }
}
