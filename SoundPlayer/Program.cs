using Microsoft.EntityFrameworkCore;
using SoundPlayer.Controllers;
using SoundPlayer.DAL;
using SoundPlayer.Extensions;

namespace SoundPlayer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddGrpc();
            builder.Services.AddGrpcReflection();

            builder.Services.AddDbContextFactory<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAuth(builder.Configuration);
            builder.Services.AddIdentityServices();
            builder.Services.AddServices();

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    await SeedData.Initialize(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.MapGrpcReflectionService();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGrpcService<AuthController>();
            //app.MapGrpcService<PlaylistController>();
            app.MapGrpcService<TrackController>();
            app.MapGrpcService<UserController>();
            
            await app.RunAsync();
        }
    }
}