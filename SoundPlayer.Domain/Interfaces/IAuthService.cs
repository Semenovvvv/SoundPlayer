using SoundPlayer.Domain.Common;
using SoundPlayer.Domain.DTO;

namespace SoundPlayer.Domain.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Регистрирует пользователя
        /// </summary>
        /// <param name="dto">Данные пользователя</param>
        /// <returns>BaseResponse(IsSuccess, Message)</returns>
        public Task<BaseResponse> RegisterUser(UserDto dto);
        
        /// <summary>
        /// Выполняет авторизацию пользователя
        /// </summary>
        /// <param name="dto">Данные пользователя</param>
        /// <returns>Токен</returns>
        public Task<BaseResponse<(UserDto, string)>> LoginUser(UserDto dto);
    }
}
