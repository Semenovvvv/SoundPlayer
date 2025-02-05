namespace SoundPlayer.Domain.DTO;

public class PlaylistDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CreatedByUserId { get; set; }
    public int TrackCount { get; set; }
}