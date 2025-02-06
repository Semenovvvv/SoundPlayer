using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SoundPlayer.DAL;
using SoundPlayer.Domain.Common;
using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Entities;
using SoundPlayer.Domain.Interfaces;

namespace SoundPlayer.Application.Services
{
    /// <inheritdoc cref="IPlaylistService"/>
    public class PlaylistService : IPlaylistService
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private readonly ILogger<PlaylistService> _logger;
        private readonly IUserService _userService;
        
        public PlaylistService(ILogger<PlaylistService> logger, 
            IDbContextFactory<AppDbContext> dbFactory,
            IUserService userService)
        {
            _logger = logger;
            _dbFactory = dbFactory;
            _userService = userService;
        }
        
        public async Task<BaseResponse<Playlist>> CreatePlaylist(int userId, string name)
        {
            try
            {
                var user = await _userService.GetUserById(userId);

                if (user == null)
                    return new BaseResponse<Playlist>()
                    {
                        IsSuccess = false,
                        Message = "User not found"
                    };

                var playlist = new Playlist()
                {
                    Name = name,
                    CreatedByUserId = user.Id
                };
                
                await using (var dbContext = await _dbFactory.CreateDbContextAsync())
                {
                    dbContext.Playlists.Add(playlist);
                    await dbContext.SaveChangesAsync();
                }

                return new BaseResponse<Playlist>()
                {
                    IsSuccess = true,
                    Message = "Playlist created",
                    Result = playlist
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Playlist didnt created. Exception = {ex.Message}");
                return new BaseResponse<Playlist>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<BaseResponse> CreateFavoritePlaylist(int userId)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync();
            var user =  await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return new BaseResponse(false, "User not found");
            
            user.FavoritePlaylist = new Playlist()
            {
                Name = "Мне нравится",
                CreatedBy = user,
                CreatedByUserId = user.Id
            };
            
            await dbContext.SaveChangesAsync();

            return new BaseResponse(true, "Playlist created");
        }
        
        public async Task<PlaylistDto?> GetPlaylist(int id)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync();
            var playlist =  await dbContext.Playlists.FirstOrDefaultAsync(x => x.Id == id);
            if (playlist == null) return null;

            var trackCount = playlist.PlaylistTracks != null ? playlist.PlaylistTracks.Count : 0;

            return new PlaylistDto()
            {
                Id = playlist.Id,
                CreatedByUserId = playlist.CreatedByUserId,
                Name = playlist.Name,
                TrackCount = trackCount
            };
        }

        public async Task<IEnumerable<Playlist>> GetPlaylistsByUserId(int userId)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync();
            return dbContext.Playlists.Where(x => x.CreatedByUserId == userId);
        }
        
        public async Task<BaseResponse> UpdatePlaylist(int id, PlaylistDto dto)
        {
            await using (var dbContext = await _dbFactory.CreateDbContextAsync())
            {
                var playlist = await dbContext.Playlists.FirstOrDefaultAsync(x => x.Id == id);

                if (playlist == null)
                    return new BaseResponse(false, "Playlist not found");
                
                playlist.Name = dto.Name;
                await dbContext.SaveChangesAsync();
            }

            return new BaseResponse(true, "Playlist updated");
        }

        public async Task<BaseResponse> AddTrackToPlaylist(int playlistId, int trackId)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync();

            var playlist = await dbContext.Playlists.FirstOrDefaultAsync(x => x.Id == playlistId)
                ?? throw new RpcException(new Status(StatusCode.NotFound, "Playlist not found"));

            var track = await dbContext.Tracks.FirstOrDefaultAsync(x => x.Id == trackId)
                ?? throw new RpcException(new Status(StatusCode.NotFound, "Track not found"));

            try
            {
                var playlistTrack = new PlaylistTrack()
                {
                    PlaylistId = playlistId,
                    TrackId = trackId
                };

                dbContext.PlaylistTracks.Add(playlistTrack);
                await dbContext.SaveChangesAsync();

                return new BaseResponse()
                {
                    IsSuccess = true,
                    Message = "Track added to playlist successfully"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<BaseResponse> DeleteTrackFromPlaylist(int playlistId, int trackId)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync();

            var playlist = await dbContext.Playlists.FirstOrDefaultAsync(x => x.Id == playlistId)
                           ?? throw new RpcException(new Status(StatusCode.NotFound, "Playlist not found"));

            var track = await dbContext.Tracks.FirstOrDefaultAsync(x => x.Id == trackId)
                        ?? throw new RpcException(new Status(StatusCode.NotFound, "Track not found"));
            
            try
            {
                var playlistTrack = await dbContext.PlaylistTracks
                    .FirstOrDefaultAsync(x => x.Playlist == playlist && x.Track == track)
                    ?? throw new RpcException(new Status(StatusCode.NotFound, "PlaylistTrack not found"));
                
                                
                dbContext.PlaylistTracks.Remove(playlistTrack);
                await dbContext.SaveChangesAsync();

                return new BaseResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Track didnt deleted from playlist. Exception = {ex.Message}");
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
