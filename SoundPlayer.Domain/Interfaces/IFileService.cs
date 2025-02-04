namespace SoundPlayer.Domain.Interfaces;

public interface IFileService
{
    public Task SaveChunkAsync(Guid fileGuid, byte[] data);

    public Task DeleteTrackAsync(Guid fileGuid, DateTime dateTime);

    public Task<string> GetTrackPath(Guid fileGuid, DateTime createdTime);
}