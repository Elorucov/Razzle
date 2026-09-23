using ELOR.Razzle.Data;

namespace ELOR.Razzle
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            SQLitePCL.Batteries_V2.Init();

            var builder = WebApplication.CreateBuilder(args);

            // Per-user encrypted SQLite files live next to the binary.
            var dataDir = Path.Combine(AppContext.BaseDirectory, "data");
            Directory.CreateDirectory(dataDir);

            builder.Services.AddSingleton(new RazzleDbContextFactory(dataDir));

            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}
