using Branca;
using System.Text.Json;

namespace ELOR.Razzle.Services
{
    public sealed record TokenPayload(string Username, string Password);

    public sealed class AccessTokenService
    {
        private readonly BrancaService _branca;
        private readonly uint _lifetimeSeconds; // 0 = non-expiring

        public AccessTokenService(IConfiguration configuration)
        {
            var hexKey = configuration["Auth:BrancaKey"]
                         ?? throw new InvalidOperationException("Auth:BrancaKey is not configured.");

            var key = Convert.FromHexString(hexKey);
            _lifetimeSeconds = configuration.GetValue("Auth:TokenLifetimeSeconds", (uint)0);

            var settings = new BrancaSettings
            {
                TokenLifetimeInSeconds = _lifetimeSeconds > 0 ? _lifetimeSeconds : null
            };

            _branca = new BrancaService(key, settings);
        }

        public (string Token, long ExpiresIn) Create(string username, string password)
        {
            var payload = JsonSerializer.SerializeToUtf8Bytes(new TokenPayload(username, password));
            var token = _branca.Encode(payload);
            var expiresIn = _lifetimeSeconds > 0
                ? DateTimeOffset.UtcNow.ToUnixTimeSeconds() + _lifetimeSeconds
                : 0L;

            return (token, expiresIn);
        }

        public TokenPayload Decode(string token)
        {
            if (!_branca.TryDecode(token, out var payload)) return null;

            try
            {
                return JsonSerializer.Deserialize<TokenPayload>(payload);
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
