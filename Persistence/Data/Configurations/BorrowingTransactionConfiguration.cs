using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class BorrowingTransactionConfiguration : IEntityTypeConfiguration<BorrowingTransaction>
    {
        public void Configure(EntityTypeBuilder<BorrowingTransaction> builder)
        {
            builder.HasKey(bt => bt.Id);


            builder.Property(bt => bt.FineAmount).HasColumnType("decimal(8,2)");


            builder.HasOne(bt => bt.Book)
                .WithMany(b => b.BorrowingTransactions)
                .HasForeignKey(bt => bt.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bt => bt.User)
                .WithMany(u => u.BorrowingTransactions)
                .HasForeignKey(bt => bt.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bt => bt.IssuedByUser)
                .WithMany(u => u.IssuedTransactions)
                .HasForeignKey(bt => bt.IssuedByUserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(bt =>  bt.BookId);
            builder.HasIndex(bt => bt.UserId);
        }
    }
}
