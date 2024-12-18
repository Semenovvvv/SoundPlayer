using SoundPlayer.Services;

namespace SoundPlayer.Extensions
{
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
