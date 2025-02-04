namespace SoundPlayer.Domain.Interfaces;

public interface IFileService
{
    public Task<bool> SaveBytes(byte[] data, bool isFinalChunk, Guid trackGuid);
    public Task GetBytes(string guid, string date, Func<byte[], Task> processChunk);
}