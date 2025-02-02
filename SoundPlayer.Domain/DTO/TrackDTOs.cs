namespace SoundPlayer.Domain.DTO
{
    public class TrackDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
