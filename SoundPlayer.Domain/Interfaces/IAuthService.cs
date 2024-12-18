using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Entities;

namespace SoundPlayer.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterUser(UserDto dto);
        Task<string> LoginUser(LoginDto dto);
    }
}
