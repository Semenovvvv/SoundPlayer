﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoundPlayer.Domain.Entities;

namespace SoundPlayer.DAL.Configurations
{
    public class TrackConfiguration:IEntityTypeConfiguration<Track>
    {
        public void Configure(EntityTypeBuilder<Track> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.UploadedByUser)
                .WithMany(x => x.UploadedTracks)
                .HasForeignKey(x => x.UploadedByUserId);

            builder.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.UniqueName)
                .HasMaxLength(300)
                .IsRequired();

            builder.Navigation(x => x.UploadedByUser)
                .AutoInclude();
        }
    }
}
