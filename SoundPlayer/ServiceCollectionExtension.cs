using SoundPlayer.Domain.Interfaces;
using SoundPlayer.Application.Services;
using IServiceCollection = Microsoft.Extensions.DependencyInjection.IServiceCollection;

namespace SoundPlayer.Presentation
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITrackService, TrackService>();

            return services;
        }
    }
}
