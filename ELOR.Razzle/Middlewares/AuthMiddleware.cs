using ELOR.Razzle.Data;
using ELOR.Razzle.Services;

namespace ELOR.Razzle.Middlewares
{
    // Checks the access token that passed in header or as "accessToken" parameter
    // If access token is valid, populates the scoped UserSession with an open database context

    public sealed class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, AccessTokenService tokens, UsersRegistry users, RazzleDbContextFactory factory, UserSession user)
        {
            var token = ExtractToken(context);
            if (!string.IsNullOrEmpty(token))
            {
                var payload = tokens.Decode(token);
                if (payload is not null)
                {
                    if (users.TryGetStorageName(payload.Username, out var storageName))
                    {
                        var db = factory.Open(storageName, payload.Password);
                        user.Set(payload.Username, db);
                    }
                }
            }

            await _next(context);
        }

        private static string ExtractToken(HttpContext context)
        {
            var authorization = context.Request.Headers.Authorization.ToString();
            if (!string.IsNullOrEmpty(authorization))
            {
                const string bearer = "Bearer ";
                return authorization.StartsWith(bearer, StringComparison.OrdinalIgnoreCase)
                    ? authorization[bearer.Length..].Trim()
                    : authorization.Trim();
            }

            if (context.Request.Query.TryGetValue("accessToken", out var fromQuery) && !string.IsNullOrEmpty(fromQuery))
            {
                return fromQuery;
            }

            if (context.Request.HasFormContentType &&
                context.Request.Form.TryGetValue("accessToken", out var fromForm) && !string.IsNullOrEmpty(fromForm))
            {
                return fromForm;
            }

            return null;
        }
    }
}
