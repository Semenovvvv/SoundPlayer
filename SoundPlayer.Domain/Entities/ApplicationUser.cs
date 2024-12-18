using Microsoft.AspNetCore.Identity;

namespace SoundPlayer.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Track>? UploadedTracks { get; set; }
        public ICollection<Playlist>? Playlists { get; set; }
    }
}
