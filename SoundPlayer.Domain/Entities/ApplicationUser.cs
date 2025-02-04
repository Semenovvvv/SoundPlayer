using Microsoft.AspNetCore.Identity;

namespace SoundPlayer.Domain.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public DateTime CreatedTime { get; set; }
        public ICollection<Track>? UploadedTracks { get; set; }
        
        public Playlist? FavoritePlaylist { get; set; }
        public ICollection<Playlist>? Playlists { get; set; }
    }
}
