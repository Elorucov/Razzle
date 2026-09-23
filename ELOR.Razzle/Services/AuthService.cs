using ELOR.Razzle.Data;

namespace ELOR.Razzle.Services
{
    public sealed class AuthService
    {
        private readonly RazzleDbContextFactory _dbFactory;

        public AuthService(RazzleDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<int> TestAsync()
        {
            await Task.Yield();
            return 42;
        }

        public async Task<int> ThrowAsync()
        {
            await Task.Yield();
            throw ServiceException.AuthFailed();
        }
    }
}
