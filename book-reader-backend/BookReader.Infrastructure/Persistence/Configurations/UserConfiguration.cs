using BookReader.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.Property(x => x.PasswordHash);

            builder.Property(x => x.ExternalId)
                .HasMaxLength(255);

            builder.HasIndex(x => x.ExternalId)
                .IsUnique()
                .HasFilter("\"ExternalId\" IS NOT NULL");

            builder.HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .IsRequired();

            builder.ToTable(t =>
                t.HasCheckConstraint("CK_User_PasswordOrExternalId",
                    "\"PasswordHash\" IS NOT NULL OR \"ExternalId\" IS NOT NULL"));
        }
    }
}
