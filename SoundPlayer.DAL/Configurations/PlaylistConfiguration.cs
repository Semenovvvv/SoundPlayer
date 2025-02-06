using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoundPlayer.Domain.Entities;

namespace SoundPlayer.DAL.Configurations
{
    public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
    {
        public void Configure(EntityTypeBuilder<Playlist> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.CreatedBy)
                .WithMany(x => x.Playlists)
                .HasForeignKey(x => x.CreatedByUserId);

            builder.Navigation(x => x.PlaylistTracks)
                .AutoInclude();
        }
    }
}
