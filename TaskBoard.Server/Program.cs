
using Microsoft.EntityFrameworkCore;
using TaskBoard.Server.Models.Mapper;
using TaskBoard.Server.Models.TaskBoardDatabase;
using TaskBoard.Server.Services;

namespace TaskBoard.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string CORSOpenPolicy = "OpenCORSPolicy";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(

                  name: CORSOpenPolicy,
                  builder =>
                  {
                      builder
                          .SetIsOriginAllowed((host) => true)/*WithOrigins("http://127.0.0.1:53956", "https://127.0.0.1:53956", "http://localhost:53956", "https://localhost:53956"*//*, Environment.GetEnvironmentVariable("FRONTEND_SERVER")??"", "*"*//*)*/
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                  });
            });

            builder.Services.AddAuthentication()
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromDays(10);
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Events.OnRedirectToAccessDenied =
                    options.Events.OnRedirectToLogin = c =>
                    {
                        c.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.FromResult<object>(null!);
                    };
                });

            builder.Services.AddDbContext<TaskBoardContext>(
                options =>
                {
                    options.UseSqlServer(Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING"), builder =>
                    {
                        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    });
                });

            builder.Services.AddAutoMapper(typeof(TaskBoardProfile).Assembly);
            builder.Services.AddTransient<ITimeProvider, Services.TimeProvider>();
            builder.Services.AddTransient<ITaskBoardDatabaseService, TaskBoardDatabaseService>();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();


            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.MapStaticAssets();

            app.UseCors(CORSOpenPolicy);

            app.UseAuthentication();
            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                var dbContextOptionsBuilder = new DbContextOptionsBuilder();
                dbContextOptionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING"), builder =>
                {
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }

            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
