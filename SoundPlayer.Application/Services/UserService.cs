using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SoundPlayer.DAL;
using SoundPlayer.Domain.Common;
using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Entities;
using SoundPlayer.Domain.Interfaces;

namespace SoundPlayer.Application.Services
{
    ///<inheritdoc cref="IUserService"/>
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public UserService(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            IConfiguration configuration,
            ILogger<UserService> logger,
            IDbContextFactory<AppDbContext> dbFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
            _dbFactory = dbFactory;
        }
        
        public async Task<ApplicationUser?> GetUserById(int userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }
        
        public async Task<BaseResponse> UpdateUserProfile(int userId, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return new BaseResponse()
                {
                    IsSuccess = false,
                    Message = "User not found"
                };

            user.UserName = dto.Username ?? user.UserName;
            user.Email = dto.Email ?? user.Email;
            var result = await _userManager.UpdateAsync(user);
            
            return new BaseResponse()
            {
                IsSuccess = result.Succeeded,
                Message = result.Errors.ToString()
            };
        }

        public async Task<BaseResponse> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) 
                return new BaseResponse()
            {
                IsSuccess = false,
                Message = "User not found"
            };

            var result = await _userManager.DeleteAsync(user);
            return new BaseResponse()
            {
                IsSuccess = result.Succeeded,
                Message = result.Errors.ToString()
            };
        }

        public async Task<PaginatedResponse<ApplicationUser>> GetUsersByName(
            string userNamePattern, 
            int pageNumber, 
            int pageSize)
        {
            try
            {
                await using var dbContext = await _dbFactory.CreateDbContextAsync();

                var query = dbContext.Users
                    .AsNoTracking()
                    .Where(user => EF.Functions.ILike(user.UserName, $"%{userNamePattern}%"));
                
                var totalRescord = await query.CountAsync();

                var tracks = await query
                    .OrderBy(t => t.UserName)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PaginatedResponse<ApplicationUser>()
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
                _logger.LogWarning($"Error while getting tracks by name '{userNamePattern}': {e.Message}");

                return new PaginatedResponse<ApplicationUser>
                {
                    IsSuccess = false,
                    Message = $"Error while getting tracks: {e.Message}"
                };
            }
        }
    }
}
