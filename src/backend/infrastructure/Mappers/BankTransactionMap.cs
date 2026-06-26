using Infrastructure.PersistenceModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Mappers
{
    public class BankTransactionMap : IEntityTypeConfiguration<BankTransactionEntity>
    {
        public void Configure(EntityTypeBuilder<BankTransactionEntity> builder)
        {
            builder.ToTable("TRANSACTIONS");

            builder.HasKey(p => p.ID);

            builder
                .Property(p => p.RowVersion)
                .IsRowVersion()
                .HasColumnName("ROW_VERSION")
                .IsRequired();

            builder.Property(p => p.ID).HasColumnName("TRANSACTION_ID");
            builder.Property(p => p.SenderFK).HasColumnName("SENDER_FK");
            builder.Property(p => p.ReceiverFK).HasColumnName("RECEIVER_FK");
            builder.Property(p => p.Amount).HasColumnName("AMOUNT");
            builder.Property(p => p.TransactionType).HasColumnName("TRANSACTION_TYPE");
            builder.Property(p => p.TransactionStatus).HasColumnName("TRANSACTION_STATUS");
            builder.Property(p => p.CreatedAt).HasColumnName("CREATED_AT");
        }
    }
}
