using ELOR.Razzle.Data;
using ELOR.Razzle.DTO.Requests;

namespace ELOR.Razzle.Services
{
    public sealed class AuthService
    {
        private readonly RazzleDbContextFactory _dbFactory;
        private readonly UsersRegistry _registry;

        public AuthService(RazzleDbContextFactory dbFactory, UsersRegistry registry)
        {
            _dbFactory = dbFactory;
            _registry = registry;
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
    }
}
