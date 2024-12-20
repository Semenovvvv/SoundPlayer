using SoundPlayer.Domain.BE;
using SoundPlayer.Domain.DTO;

namespace SoundPlayer.Domain.Interfaces
{
    public interface ITrackService
    {
        public Task SaveTrackChunkInDirectory(TrackChunk audioChunk, Guid trackGuid);
        public Task<BaseResponse> SaveTrackInfo(TrackDto dto);

        public Task GetTrackChunks(int trackId, Func<byte[], Task> processChunk);
        public Task<TrackDto> GetTrackInfo(int id);
    }
}
