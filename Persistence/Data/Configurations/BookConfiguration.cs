using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Data.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("books");
            builder.HasKey(b => new {b.Id, b.ISBN });

            builder.Property(b => b.ISBN).IsRequired().HasMaxLength(20);
            builder.HasIndex(b => b.ISBN).IsUnique();

            builder.Property(b => b.Title).IsRequired().HasMaxLength(300);
            builder.Property(b => b.Edition).HasMaxLength(50);
            builder.Property(b => b.Summary).HasColumnType("text");


            builder.HasOne(b => b.Language)
                .WithMany(l => l.Books)
                .HasForeignKey(b => b.LanguageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
