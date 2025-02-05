namespace SoundPlayer.Domain.Entities
{
    public class Track : ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UniqueName { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public int UploadedByUserId { get; set; }
        public ApplicationUser? UploadedByUser { get; set; }
        
        public object Clone() => this.MemberwiseClone();
    }
}
