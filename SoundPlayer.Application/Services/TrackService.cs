using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SoundPlayer.DAL;
using SoundPlayer.Domain.Common;
using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Entities;
using SoundPlayer.Domain.Interfaces;

namespace SoundPlayer.Application.Services
{
    public class TrackService : ITrackService
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly ILogger<TrackService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileService;

        public TrackService(
            IDbContextFactory<AppDbContext> dbContextFactory,
            ILogger<TrackService> logger,
            IFileService fileService,
            UserManager<ApplicationUser> userManager)
        {
            _fileService = fileService;
            _userManager = userManager;
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }
        
        public async Task<BaseResponse<Track>> SaveTrackInfo(TrackDto dto, Guid uniqueName, DateTime createDate)
        {
            try
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

                var user = await _userManager.FindByIdAsync(dto.UserId.ToString());

                if (user is null)
                {
                    _logger.LogWarning($"Track information {dto.Name} don't loaded. User not found");
                    return new BaseResponse<Track>
                    {
                        IsSuccess = false,
                        Message = $"Track information {dto.Name} don't loaded. User not found"
                    };
                }

                var track = new Track()
                {
                    Name = dto.Name,
                    UploadDate = createDate,
                    UploadedByUserId = user.Id,
                    Duration = dto.Duration,
                    UniqueName = uniqueName.ToString()
                };

                var trackCopy = (Track)track.Clone();

                await dbContext.Tracks.AddAsync(track);
                await dbContext.SaveChangesAsync();

                _logger.LogInformation($"Track information {track.Name} was loaded");
                
                return new BaseResponse<Track>
                {
                    IsSuccess = true,
                    Message = $"Track information {track.Name} was loaded",
                    Result = track
                };
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Track information {dto.Name} wasn't loaded. Exception : {e.Message}");

                return new BaseResponse<Track>
                {
                    IsSuccess = false,
                    Message = $"Track information {dto.Name} wasn't loaded. Exception : {e.Message}"
                };
            }
        }

        public async Task<BaseResponse<TrackDto>> GetTrackInfo(int id)
        {
            try
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
                var track = await dbContext.Tracks
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (track is null)
                {
                    throw new Exception("Track not found");
                }

                return new BaseResponse<TrackDto>()
                {
                    IsSuccess = true,
                    Result = new TrackDto()
                    {
                        Name = track.Name,
                        UserEmail = track.UploadedByUser?.Email ?? "",
                        UserName = track.UploadedByUser?.UserName ?? "",
                        Duration = track.Duration,
                        UniqueName = track.UniqueName,
                        UploadDate = track.UploadDate
                    }
                };
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Track 'trackId = {id}' not getted. Exception : {e.Message}");
                return new BaseResponse<TrackDto>()
                {
                    Message = e.Message,
                    IsSuccess = false
                };
            }
        }
        
        public async Task<PaginatedResponse<TrackDto>> GetTrackListByName(
            string trackName, 
            int pageNumber, 
            int pageSize)
        {
            try
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

                var query = dbContext.Tracks
                    .AsNoTracking()
                    .Where(t => EF.Functions.ILike(t.Name, $"%{trackName}%"));

                var totalRescord = await query.CountAsync();

                var tracks = await query
                    .OrderBy(t => t.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new TrackDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        UserEmail = t.UploadedByUser.Email,
                        UserName = t.UploadedByUser.UserName,
                        Duration = t.Duration
                    })
                    .ToListAsync();

                return new PaginatedResponse<TrackDto>()
                {
                    IsSuccess = true,
                    
                    Items = tracks,
                    TotalCount = totalRescord,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

            }
            catch (Exception e)
            {
                _logger.LogWarning($"Error while getting tracks by name '{trackName}': {e.Message}");

                return new PaginatedResponse<TrackDto>
                {
                    IsSuccess = false,
                    Message = $"Error while getting tracks: {e.Message}"
                };
            }
        }
    }
}
