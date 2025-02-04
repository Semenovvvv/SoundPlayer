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

        private readonly Dictionary<Guid, FileStream> _activeStreams = new();

        public bool FileExists(string guid) => File.Exists(@$"{_trackPath}\{guid}");

        public async Task<bool> SaveBytes(byte[] data, bool isFinalChunk, Guid trackGuid)
        {
            var directoryPath = @$"{_trackPath}\{DateTime.UtcNow:yyyy_MM_dd}";

            if (!Directory.Exists(_trackPath.ToString()))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!_activeStreams.ContainsKey(trackGuid))
            {
                var uniqueFileName = $"{trackGuid}";
                var filePath = Path.Combine(directoryPath, uniqueFileName);
                _activeStreams[trackGuid] = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            }

            var fileStream = _activeStreams[trackGuid];
            await fileStream.WriteAsync(data, 0, data.Length);

            if (isFinalChunk)
            {
                await fileStream.DisposeAsync();
                _activeStreams.Remove(trackGuid);
                return true;
            }

            return false;
        }

        public async Task GetBytes(string guid, string date,  Func<byte[], Task> processChunk)
        {
            var chunkSize = 8192;

            using var fileStream = new FileStream(@$"{_trackPath}\{date}\{guid}", FileMode.Open, FileAccess.Read);
            var buffer = new byte[chunkSize];
            int bytesRead;

            while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                var chunk = bytesRead == buffer.Length ? buffer : buffer.Take(bytesRead).ToArray();
                await processChunk(chunk);
            }
        }
    }
}
