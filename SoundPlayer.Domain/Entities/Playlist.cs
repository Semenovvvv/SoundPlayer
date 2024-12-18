namespace SoundPlayer.Domain.Entities
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedByUserId { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public ICollection<PlaylistTrack> PlaylistTracks { get; set; }
    }
}
