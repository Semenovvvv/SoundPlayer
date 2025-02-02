using SoundPlayer.Domain.BE;
using SoundPlayer.Domain.DTO;

namespace SoundPlayer.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse<bool>> RegisterUser(UserDto dto);
        Task<BaseResponse<(UserDto, string)>> LoginUser(UserDto dto);
    }
}
