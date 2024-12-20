namespace SoundPlayer.Domain.DTO
{
    public class TrackChunk
    {
        public bool IsFinishChunk { get; set; }
        public byte[] Data { get; set; }
    }
}
