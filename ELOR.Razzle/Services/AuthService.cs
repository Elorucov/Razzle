using ELOR.Razzle.Data;
using ELOR.Razzle.DTO.Requests;
using ELOR.Razzle.DTO.Responses;
using ELOR.Razzle.Services.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ELOR.Razzle.Services
{
    public sealed class AuthService
    {
        private readonly RazzleDbContextFactory _dbFactory;
        private readonly UsersRegistry _registry;
        private readonly AccessTokenService _tokens;

        public AuthService(RazzleDbContextFactory dbFactory, UsersRegistry registry, AccessTokenService tokens)
        {
            _dbFactory = dbFactory;
            _registry = registry;
            _tokens = tokens;
        }

        // TODO: more restricted registration (captcha?)
        public async Task<bool> SignUpAsync(SignUpRequest request)
        {
            var username = request.Username;
            if (_registry.Exists(username) || _dbFactory.Exists(username))
            {
                throw ServiceException.UserAlreadyExists();
            }

            var userStorageName = _registry.Add(username);
            using var db = _dbFactory.Create(userStorageName, request.Password);

            return true;
        }

        public async Task<SignInResponse> SignInAsync(SignInRequest request)
        {
            var username = request.Username;
            var password = request.Password;

            if (!_registry.TryGetStorageName(username, out string storageName))
            {
                throw ServiceException.AuthFailed();
            }

            VerifyDatabase(storageName, password);

            var (token, expiresIn) = _tokens.Create(username, password);

            return new SignInResponse
            {
                AccessToken = token,
                ExpiresIn = expiresIn
            };
        }

        // Opens the user's encrypted database to confirm the password.
        private void VerifyDatabase(string storageName, string password)
        {
            var db = _dbFactory.Open(storageName, password);
            try
            {
                var connection = db.Database.GetDbConnection();
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT count(*) FROM sqlite_master;";
                command.ExecuteScalar();
            }
            catch (SqliteException ex) when (IsWrongKey(ex))
            {
                throw ServiceException.AuthFailed();
            }
            finally
            {
                db.Dispose();
            }
        }

        // Strange way to check wrong DB file
        private static bool IsWrongKey(SqliteException ex) =>
            ex.SqliteErrorCode == 26
            || ex.Message.Contains("not a database", StringComparison.OrdinalIgnoreCase)
            || ex.Message.Contains("encrypted", StringComparison.OrdinalIgnoreCase);
    }
}
