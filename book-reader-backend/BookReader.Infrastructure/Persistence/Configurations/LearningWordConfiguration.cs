using BookReader.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Infrastructure.Persistence.Configurations
{
    public class LearningWordConfiguration : IEntityTypeConfiguration<LearningWord>
    {
        public void Configure(EntityTypeBuilder<LearningWord> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Translation)
               .WithMany()
               .HasForeignKey(x => x.TranslationId)
               .IsRequired();

            builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .IsRequired();

            builder.HasIndex(x => new
            {
                x.UserId,
                x.TranslationId
            });
        }
    }
}
