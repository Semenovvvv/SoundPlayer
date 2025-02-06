using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SoundPlayer.Domain.Interfaces;

namespace SoundPlayer.Application.Services
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;
        private readonly string _trackPath;
        
        public FileService(IConfiguration configuration, IHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
            _trackPath = Path.Combine(_environment.ContentRootPath, _configuration["TracksDirectory"] ?? throw new InvalidOperationException("TracksDirectory is not configured."));
        }
        
        public async Task SaveChunkAsync(Guid fileGuid, byte[] data)
        {
            var directoryPath = @$"{_trackPath}\{DateTime.UtcNow:yyyy_MM_dd}";
            
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            
            var filePath = Path.Combine(_trackPath, $"{DateTime.UtcNow:yyyy_MM_dd}", fileGuid.ToString());
            await using var stream = new FileStream(filePath, FileMode.Append);
            await stream.WriteAsync(data);
        }
        
        public async Task DeleteTrackAsync(Guid fileGuid, DateTime dateTime)
        {
            var filePath = Path.Combine(_trackPath, $"{dateTime:yyyy_MM_dd}", fileGuid.ToString());
            File.Delete(filePath);
        }

        public async Task<string> GetTrackPath(Guid fileGuid, DateTime createdTime) =>
            Path.Combine(_trackPath, $"{createdTime:yyyy_MM_dd}", fileGuid.ToString());
    }
}
