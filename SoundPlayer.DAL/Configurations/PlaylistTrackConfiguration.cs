using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoundPlayer.Domain.Entities;

namespace SoundPlayer.DAL.Configurations
{
    public class PlaylistTrackConfiguration : IEntityTypeConfiguration<PlaylistTrack>
    {
        public void Configure(EntityTypeBuilder<PlaylistTrack> builder)
        {
            builder.HasKey(x => new { x.PlaylistId, x.TrackId});

            builder.HasOne(x => x.Playlist)
                .WithMany(x => x.PlaylistTracks)
                .HasForeignKey(x => x.PlaylistId);

            builder.HasOne(x => x.Track)
                .WithMany()
                .HasForeignKey(x => x.TrackId);
        }
    }
}
