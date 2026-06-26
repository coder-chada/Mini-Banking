using Infrastructure.PersistenceModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Mappers
{
    public class IdempotencyMap : IEntityTypeConfiguration<IdempotencyEntity>
    {
        public void Configure(EntityTypeBuilder<IdempotencyEntity> builder)
        {
            builder.ToTable("IDEMPOTENCY_KEYS");

            builder.HasKey(p => p.idempotency_key);

            builder.Property(p => p.idempotency_key)
                   .HasColumnName("IDEMPOTENCY_KEY");

            builder.Property(p => p.request_hash)
                   .HasColumnName("REQUEST_HASH");

            builder.Property(p => p.status)
                   .HasColumnName("STATUS");

            builder.Property(p => p.response_body)
                   .HasColumnName("RESPONSE_BODY");

            builder.Property(p => p.status_code)
                   .HasColumnName("STATUS_CODE");

            builder.Property(p => p.created_at)
                   .HasColumnName("CREATED_AT");

            builder.Property(p => p.completed_at)
                   .HasColumnName("COMPLETED_AT");

            builder.Property(p => p.expires_at)
                   .HasColumnName("EXPIRES_AT");
        }
    }
}
