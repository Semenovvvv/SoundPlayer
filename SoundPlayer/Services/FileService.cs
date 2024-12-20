namespace SoundPlayer.Application.Services
{
    public class FileService
    {
        private Uri _defaultUri = new Uri(@"C:\Users\leoni\source\repos\Sounds");
        private string _defaultPath = @"C:\Users\leoni\source\repos\Sounds";

        private readonly Dictionary<Guid, FileStream> _activeStreams = new();

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
    }
}
