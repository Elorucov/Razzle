using ELOR.Razzle.API;
using ELOR.Razzle.Controllers;
using ELOR.Razzle.Data;
using ELOR.Razzle.Services;
using FluentValidation;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ELOR.Razzle
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            SQLitePCL.Batteries_V2.Init();

            // Per-user encrypted SQLite files live next to the binary
            var dataDir = Path.Combine(AppContext.BaseDirectory, "data");
            Directory.CreateDirectory(dataDir);

            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddControllers(options =>
                {
                    options.Conventions.Add(new VKAPIStyleRouteConvention());
                    options.Filters.Add<ValidationFilter>();
                    options.Filters.Add<APIResultFilter>();
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            // FluentValidation property names are reported in camelCase
            ValidatorOptions.Global.PropertyNameResolver = (_, member, _) =>
                member is null ? null : char.ToLowerInvariant(member.Name[0]) + member.Name[1..];

            builder.Services.AddSingleton(new RazzleDbContextFactory(dataDir));
            builder.Services.AddScoped<AuthService>();

            var app = builder.Build();

            app.UseMiddleware<APIExceptionMiddleware>();
            app.UseRouting();

            app.MapControllers();

            app.MapFallback("/{**path}", (HttpContext context) =>
                APIResults.WriteError(context,
                    new ApiError { Code = ErrorCodes.UnknownMethod, Message = "Unknown method passed" },
                    StatusCodes.Status404NotFound, context.RequestAborted));

            app.Run();
        }
    }
}
