using SoundPlayer.Domain.Common;
using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Entities;

namespace SoundPlayer.Domain.Interfaces
{
    public interface IPlaylistService
    {
        /// <summary>
        /// Создает плейлист пользователя
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <param name="name">Название плейлиста</param>
        /// <returns>BaseResponse'PlaylistDto'</returns>
        public Task<BaseResponse<Playlist>> CreatePlaylist(int userId, string name);

        /// <summary>
        /// Создает плейлист с любимыми треками
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns>BaseResponse(IsSuccess, Message)</returns>
        public Task<BaseResponse> CreateFavoritePlaylist(int userId);
        
        /// <summary>
        /// Выполняет получение плейлиста по Id
        /// </summary>
        /// <param name="id">Id плейлиста</param>
        /// <returns>Playlist</returns>
        public Task<PlaylistDto?> GetPlaylist(int id);

        /// <summary>
        /// Выполняет получение коллекции плейлистов пользователя
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns>Коллекция плейлистов</returns>
        public Task<IEnumerable<Playlist>> GetPlaylistsByUserId(int userId);

        /// <summary>
        /// Выполняет обновление плейлиста
        /// </summary>
        /// <param name="id">Id плейлиста</param>
        /// <param name="dto">Данные для изменения</param>
        /// <returns>BaseResponse(IsSuccess, Message)</returns>
        public Task<BaseResponse> UpdatePlaylist(int id, PlaylistDto dto);
    }
}
