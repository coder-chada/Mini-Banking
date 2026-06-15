using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.PersistenceModels;

namespace Infrastructure.Mappers
{
    internal class AccountMap : IEntityTypeConfiguration<AccountEntity>
    {
        public void Configure(EntityTypeBuilder<AccountEntity> builder)
        {
            builder.ToTable("ACCOUNTS");

            builder.HasKey(p => p.ID);

            builder.Property(p => p.ID)
                .HasColumnName("ACCOUNT_ID")
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            builder.Property(p => p.Numero)
                .HasColumnName("NUMERO_CUENTA");

            builder.Property(p => p.Tipo)
                .HasColumnName("TIPO_CUENTA")
                .HasConversion<int>();

            builder.Property(p => p.Currency)
                .HasColumnName("TIPO_CURRENCY")
                .HasConversion<int>();

            builder.Property(p => p.Balance)
                .HasColumnName("BALANCE");

            builder.Property(p => p.OwnerID)
                .HasColumnName("USUARIO_FK");
        }
    }
}
