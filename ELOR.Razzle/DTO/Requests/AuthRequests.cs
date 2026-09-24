namespace ELOR.Razzle.DTO.Requests
{
    public sealed class SignInRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    // TODO: additional fields?
    public sealed class SignUpRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
