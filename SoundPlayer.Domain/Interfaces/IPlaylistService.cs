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

        /// <summary>
        /// Добавить трек в плейлист
        /// </summary>
        /// <param name="playlistId">Id плейлиста</param>
        /// <param name="trackId">Id трека</param>
        /// <returns>BaseResponse(IsSuccess, Message)</returns>
        public Task<BaseResponse> AddTrackToPlaylist(int playlistId, int trackId);

        /// <summary>
        /// Удалить трек из плейлиста
        /// </summary>
        /// <param name="playlistId">Id плейлиста</param>
        /// <param name="trackId">Id трека</param>
        /// <returns>BaseResponse</returns>
        public Task<BaseResponse> DeleteTrackFromPlaylist(int playlistId, int trackId);
        
        public Task<PaginatedResponse<Track>> GetTrackList(int playlistId, int pageSize, int pageNumber);
    }
}
