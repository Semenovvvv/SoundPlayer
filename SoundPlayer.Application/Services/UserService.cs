using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
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
    }
}
