using SoundPlayer.Domain.BE;
using SoundPlayer.Domain.DTO;

namespace SoundPlayer.Domain.Interfaces
{
    public interface ITrackService
    {
        public Task SaveTrackChunkInDirectory(TrackChunk audioChunk, Guid trackGuid);
        public Task<BaseResponse> SaveTrackInfo(TrackDto dto);
    }
}
