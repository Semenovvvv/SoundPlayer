using SoundPlayer.Application.Services;
using SoundPlayer.Domain.Interfaces;
using IServiceCollection = Microsoft.Extensions.DependencyInjection.IServiceCollection;

namespace SoundPlayer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITrackService, TrackService>();

            return services;
        }
    }
}
