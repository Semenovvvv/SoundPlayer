using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Entities;

namespace SoundPlayer.Domain.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserProfile(string userId);
        Task<bool> UpdateUserProfile(string userId, UpdateUserDto dto);
        Task<bool> DeleteUser(string userId);
    }
}
