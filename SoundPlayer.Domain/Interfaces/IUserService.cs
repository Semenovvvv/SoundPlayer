using SoundPlayer.Domain.Common;
using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Entities;

namespace SoundPlayer.Domain.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Выполняет поиск пользователя по Id
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns>Сущность пользователя или null</returns>
        public Task<ApplicationUser?> GetUserById(int userId);

        /// <summary>
        /// Выполняет обновление данных пользователя по Id
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <param name="dto">Данные для обновления</param>
        /// <returns>BaseResponse(IsSuccess, Message)</returns>
        public Task<BaseResponse> UpdateUserProfile(int userId, UpdateUserDto dto);

        /// <summary>
        /// Выполняет удаление пользователя по Id
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns>BaseResponse(IsSuccess, Message)</returns>
        public Task<BaseResponse> DeleteUser(string userId);

        /// <summary>
        /// Получить список пользователей по паттерну userName
        /// </summary>
        /// <param name="userNamePattern">Паттерн поиска</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <returns>Пагинационный список пользователей</returns>
        public Task<PaginatedResponse<ApplicationUser>> GetUsersByName(
            string userNamePattern,
            int pageNumber,
            int pageSize);
    }
}
