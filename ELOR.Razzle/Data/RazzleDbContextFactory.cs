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

        public string PathFor(string storageName) => Path.Combine(_dataDir, storageName + ".db");

        public bool Exists(string storageName) => File.Exists(PathFor(storageName));

        // Opens a context without touching the schema. The encryption key is applied via the
        // SQLite3MC "Password" connection-string keyword.
        public RazzleDbContext Open(string storageName, string password)
        {
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = PathFor(storageName),
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
        public RazzleDbContext Create(string storageName, string password)
        {
            var context = Open(storageName, password);
            context.Database.EnsureCreated();
            return context;
        }
    }
}
