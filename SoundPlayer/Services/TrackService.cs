using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoundPlayer.Application.Services;
using SoundPlayer.DAL;
using SoundPlayer.Domain.BE;
using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Entities;
using SoundPlayer.Domain.Interfaces;

namespace SoundPlayer.Services
{
    public class TrackService : ITrackService
    {
        private readonly FileService _fileService;
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;



        public TrackService(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _fileService = new FileService();
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task SaveTrackChunkInDirectory(TrackChunk audioChunk, Guid trackGuid)
        {
            await _fileService.SaveBytes(audioChunk.Data, audioChunk.IsFinishChunk, trackGuid);
        }

        public async Task<BaseResponse> SaveTrackInfo(TrackDto dto)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == dto.UploadedByUserId);

            if (user == null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Не удалось загрузить информацию о треке. Пользователь отсутствует."
                };
            }

            var track = new Track()
            {
                Title = dto.Title,
                UploadDate = DateTime.UtcNow,
                UploadedByUserId = dto.UploadedByUserId,
            };

            await _dbContext.Tracks.AddAsync(track);
            await _dbContext.SaveChangesAsync();

            return new BaseResponse
            {
                IsSuccess = true,
                Message = "Информация о треке успешно загружена."
            };
        }
    }
}
