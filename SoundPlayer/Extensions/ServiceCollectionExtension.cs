using SoundPlayer.Domain.Interfaces;
using SoundPlayer.Services;

namespace SoundPlayer.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            //services.AddTransient<AuthService>();
            //services.AddTransient<PlaylistService>();
            //services.AddTransient<TrackService>();
            //services.AddTransient<UserService>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITrackService, TrackService>();

            return services;
        }
    }
}
