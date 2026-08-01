using BookReader.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Infrastructure.Persistence.Configurations
{
    public class TranslationConfiguration : IEntityTypeConfiguration<Translation>
    {
        public void Configure(EntityTypeBuilder<Translation> builder)
        {
            builder.ToTable("translations");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.SourceLang)
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(x => x.TargetLang)
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(x => x.SourceWord)
                .IsRequired();

            builder.Property(x => x.TranslatedWord)
                .IsRequired();

            builder.HasIndex(x => new { x.SourceLang, x.TargetLang, x.SourceWord })
                .HasDatabaseName("IX_Translations_Search")
                .IsUnique();
        }
    }
}
