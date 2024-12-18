namespace SoundPlayer.Domain.Entities
{
    public class Track
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public string UploadedByUserId { get; set; }
        public ApplicationUser UploadedByUser { get; set; }

        public void UpdateTitle(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("Title cannot be empty.");

            Title = newTitle;
        }
    }
}
