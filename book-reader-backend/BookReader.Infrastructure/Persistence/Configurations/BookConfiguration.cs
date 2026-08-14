using BookReader.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BookReader.Infrastructure.Persistence.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("books");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OriginalFileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.StoragePath)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.ParsedFilesPath)
                .HasMaxLength(500);

            builder.Property(x => x.FileSize)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.HasIndex(x => x.UserId);
        }
    }
}
