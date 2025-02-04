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

        public async Task SaveTrackChunkInDirectory(TrackChunk audioChunk, Guid trackGuid)
        {
            await _fileService.SaveBytes(audioChunk.Data, audioChunk.IsFinishChunk, trackGuid);
        }

        public async Task<BaseResponse> SaveTrackInfo(TrackDto dto)
        {
            try
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

                var user = await _userManager.FindByEmailAsync(dto.UserEmail);

                if (user is null)
                {
                    _logger.LogWarning($"Track information {dto.Name} don't loaded. User not found");
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = $"Track information {dto.Name} don't loaded. User not found"
                    };
                }

                var track = new Track()
                {
                    Name = dto.Name,
                    UploadDate = DateTime.UtcNow,
                    UploadedByUserId = user.Id,
                    Duration = dto.Duration,
                    FilePath = ""
                };

                await dbContext.Tracks.AddAsync(track);
                await dbContext.SaveChangesAsync();

                _logger.LogInformation($"Track information {track.Name} was loaded");

                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = $"Track information {track.Name} was loaded"
                };
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Track information {dto.Name} wasn't loaded. Exception : {e.Message}");

                return new BaseResponse
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
                        UserEmail = track.UploadedByUser.Email,
                        Duration = track.Duration
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

        public async Task GetTrackChunks(int trackId, Func<byte[], Task> processChunk)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var track = await dbContext.Tracks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == trackId);
            //if (track == null)
            //{
            //    throw new FileNotFoundException($"Трек с ID {trackId} не найден.");
            //}

            //if (!_fileService.FileExists(track.FilePath))
            //{
            //    throw new FileNotFoundException($"Файл трека не найден: {track.FilePath}");
            //}

            await _fileService.GetBytes(track.FilePath, track.UploadDate.ToString("yyyy_MM_dd"), processChunk);
        }

        public async Task<BaseResponse<PaginatedResponse<TrackDto>>> GetTrackListByName(
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

                return new BaseResponse<PaginatedResponse<TrackDto>>()
                {
                    IsSuccess = true,
                    Result = new PaginatedResponse<TrackDto>()
                    {
                        Items = tracks,
                        TotalCount = totalRescord,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                };

            }
            catch (Exception e)
            {
                _logger.LogWarning($"Error while getting tracks by name '{trackName}': {e.Message}");

                return new BaseResponse<PaginatedResponse<TrackDto>>
                {
                    IsSuccess = false,
                    Message = $"Error while getting tracks: {e.Message}"
                };
            }
        }

        public async Task SaveTrackInfoAsync(TrackDto trackInfo, string filePath)
        {
            //var userId = await _userManager.FindByIdAsync(trackInfo.UserEmail);
            //if (userId == null)
            //{
            //    throw new Exception($"User with email {trackInfo.UserEmail} not found.");
            //}

            //var track = new Track
            //{
            //    Name = trackInfo.Name,
            //    UploadedByUserId = trackInfo.UserId,
            //    Duration = trackInfo.Duration,
            //    FilePath = filePath,
            //    UploadDate = DateTime.UtcNow
            //};

            //await _trackRepository.SaveTrackAsync(track);
        }
    }
}
