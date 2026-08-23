using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Persistence.Data.Configurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {

            builder.HasKey(a => a.Id);

            builder.Property(a => a.FullName).IsRequired();
            builder.Property(a => a.Bio).HasColumnType("text");

        }
    }
}
