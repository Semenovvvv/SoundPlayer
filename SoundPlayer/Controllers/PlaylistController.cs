using Grpc.Core;
using SoundPlayer.Domain.Interfaces;

namespace SoundPlayer.Controllers
{
    public class PlaylistController : PlaylistProto.PlaylistProtoBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        public override async Task<CreatePlaylistResponse> CreatePlaylist(CreatePlaylistRequest request, ServerCallContext context)
        {
            var userId = request.UserId;
            var playlistName = request.Name;

            var result = await _playlistService.CreatePlaylist(userId, playlistName);
            var playlist = result.Result ?? throw new RpcException(new Status(StatusCode.Aborted, result.Message));

            var response = new CreatePlaylistResponse()
            {
                IsSuccess = result.IsSuccess,
                Name = playlist.Name,
                Id = playlist.Id
            };

            return response;
        }

        public override async Task<GetPlaylistResponse> GetPlaylist(PlaylistId request, ServerCallContext context)
        {
            var id = request.Id;

            var playlist = await _playlistService.GetPlaylist(id) 
                           ?? throw new RpcException(new Status(StatusCode.Unknown, "Playlist not found"));

            return new GetPlaylistResponse()
            {
                PlaylistId = id,
                PlaylistName = playlist.Name,
                TrackCount = playlist.TrackCount,
                IsSuccess = true
            };
        }

        public override async Task<AddTrackResponse> AddTrackToPlaylist(AddTrackRequest request, ServerCallContext context)
        {
            var playlistId = request.PlaylistId;
            var trackId = request.TrackId;

            var result = await _playlistService.AddTrackToPlaylist(playlistId, trackId);

            var response = new AddTrackResponse()
            {
                Success = result.IsSuccess
            };

            return response;
        }

        public override async Task<DeleteTrackRes> DeleteTrackFromPlaylist(DeleteTrackReq request, ServerCallContext context)
        {
            var playlistId = request.PlaylistId;
            var trackId = request.TrackId;

            var result = await _playlistService.DeleteTrackFromPlaylist(playlistId, trackId);
            return new DeleteTrackRes()
            {
                Success = result.IsSuccess
            };
        }

        public override async Task<GetTrackListResponse> GetTrackList(GetTrackListRequest request, ServerCallContext context)
        {
            var playlistId = request.PlaylistId;
            var pageSize = request.PageSize;
            var pageNumber = request.PageNumber;

            var tracks = await _playlistService.GetTrackList(playlistId, pageSize, pageNumber);

            return new GetTrackListResponse()
            {
                TotalCount = tracks.TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Tracks =
                {
                    tracks.Items.Select(x => new TrackMetadata()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        UserId = x.UploadedByUserId,
                        DurationInSeconds = x.Duration.Seconds,
                        UserName = x.UploadedByUser?.UserName ?? string.Empty
                    })
                }
            };
        }
    }
}
