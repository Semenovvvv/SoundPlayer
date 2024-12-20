using SoundPlayer.Domain.DTO;

namespace SoundPlayer.Application.Services
{
    public class FileService
    {
        private Uri _defaultUri = new Uri(@"C:\Users\leoni\source\repos\Sounds");
        private string _defaultPath = @"C:\Users\leoni\source\repos\Sounds";

        private readonly Dictionary<Guid, FileStream> _activeStreams = new();

        public bool FileExists(string guid) => File.Exists(@$"{_defaultPath}\{guid}");

        public async Task<bool> SaveBytes(byte[] data, bool isFinalChunk, Guid trackGuid)
        {
            var directoryPath = @$"{_defaultPath}\{DateTime.UtcNow:yyyy_MM_dd}";

            if (!Directory.Exists(_defaultUri.ToString()))
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
            //if (!FileExists(guid))
            //    throw new FileNotFoundException($"Файл не найден: {guid}");

            var chunkSize = 8192;

            using var fileStream = new FileStream(@$"{_defaultPath}\{date}\{guid}", FileMode.Open, FileAccess.Read);
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
