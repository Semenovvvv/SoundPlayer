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
        
        public async Task<BaseResponse> CreatePlaylist(int userId, string name)
        {
            var user = await _userService.GetUserById(userId);

            if (user == null)
                return new BaseResponse(false, "User not found");

            await using (var dbContext = await _dbFactory.CreateDbContextAsync())
            {
                dbContext.Playlists.Add(new Playlist()
                {
                    Name = name,
                    CreatedBy = user,
                    CreatedByUserId = user.Id
                });
                await dbContext.SaveChangesAsync();
            }

            return new BaseResponse(true, "Playlist created");
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
        
        public async Task<Playlist?> GetPlaylist(int id)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync();
            return await dbContext.Playlists.FirstOrDefaultAsync(x => x.Id == id);
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
    }
}
