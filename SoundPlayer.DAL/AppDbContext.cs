using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoundPlayer.DAL.Configurations;
using SoundPlayer.Domain.Entities;

namespace SoundPlayer.DAL
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public DbSet<ApplicationUser> Users { get; init; }
        public DbSet<Track> Tracks { get; init; }
        public DbSet<Playlist> Playlists { get; init; }
        
        public DbSet<PlaylistTrack> PlaylistTracks { get; init; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.ApplyConfiguration(new PlaylistConfiguration());
            builder.ApplyConfiguration(new PlaylistTrackConfiguration());
            builder.ApplyConfiguration(new TrackConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
