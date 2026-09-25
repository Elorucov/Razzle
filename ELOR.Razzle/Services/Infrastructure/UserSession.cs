using ELOR.Razzle.Data;

namespace ELOR.Razzle.Services.Infrastructure
{
    public sealed class UserSession : IDisposable
    {
        public string Username { get; private set; } = string.Empty;
        public RazzleDbContext DB { get; private set; }

        public bool IsAuthenticated => DB is not null;

        public void Set(string login, RazzleDbContext db)
        {
            Username = login;
            DB = db;
        }

        public void Dispose()
        {
            DB?.Dispose();
        }
    }
}
