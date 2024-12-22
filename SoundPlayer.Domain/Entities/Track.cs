namespace SoundPlayer.Domain.Entities
{
    public class Track
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public int UploadedByUserId { get; set; }
        public ApplicationUser? UploadedByUser { get; set; }
    }
}
