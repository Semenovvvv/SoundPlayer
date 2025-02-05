using SoundPlayer.Domain.Common;
using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Entities;

namespace SoundPlayer.Domain.Interfaces
{
    public interface ITrackService
    {
        public Task<BaseResponse<Track>> SaveTrackInfo(TrackDto dto, Guid uniqueName, DateTime createDate);
        
        public Task<BaseResponse<TrackDto>> GetTrackInfo(int id);

        public Task<PaginatedResponse<TrackDto>> GetTrackListByName(
            string trackName,
            int pageNumber,
            int pageSize);
    }
}
