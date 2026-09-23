using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ELOR.Razzle.Data
{
    public sealed class RazzleDbContextFactory
    {
        private readonly string _dataDir;

        public RazzleDbContextFactory(string dataDir)
        {
            _dataDir = dataDir;
        }

        public string PathFor(string login) => Path.Combine(_dataDir, login.ToLowerInvariant() + ".db");

        public bool Exists(string login) => File.Exists(PathFor(login));

        // Opens a context without touching the schema. The encryption key is applied via the
        // SQLite3MC "Password" connection-string keyword.
        public RazzleDbContext Open(string login, string password)
        {
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = PathFor(login),
                Mode = SqliteOpenMode.ReadWriteCreate,
                Password = password,
                Pooling = false
            }.ToString();

            var options = new DbContextOptionsBuilder<RazzleDbContext>()
                .UseSqlite(connectionString)
                .Options;

            return new RazzleDbContext(options);
        }

        // Opens a context and ensures the schema exists (used on sign-up).
        public RazzleDbContext Create(string login, string password)
        {
            var context = Open(login, password);
            context.Database.EnsureCreated();
            return context;
        }
    }
}
