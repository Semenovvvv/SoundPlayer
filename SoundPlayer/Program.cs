using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SoundPlayer.DAL;
using SoundPlayer.Domain.Interfaces;
using SoundPlayer.Services;

namespace SoundPlayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddGrpc();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddServices();

            builder.Services.AddScoped<IAuthService, AuthService>();

            var app = builder.Build();

            // Middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGrpcService<AuthGrpcService>();
            app.MapGrpcService<PlaylistGrpcService>();
            app.MapGrpcService<TrackGrpcService>();
            app.MapGrpcService<UserGrpcService>();

            app.Run();
        }
    }

    public static class ServiceProviderExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<AuthService>();
            services.AddTransient<PlaylistService>();
            services.AddTransient<TrackService>();
            services.AddTransient<UserService>();
        }
    }
}