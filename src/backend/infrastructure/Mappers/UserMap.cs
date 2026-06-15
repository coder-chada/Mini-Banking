using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.PersistenceModels;

namespace Infrastructure.Mappers
{
    internal class UserMap : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("USUARIOS", "DEV");

            builder.HasKey(p => p.ID); 

            builder.Property(p => p.ID)
                .HasColumnName("USUARIO_ID")
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            builder.Property(p => p.DNI)
                .HasColumnName("DNI");

            builder.Property(p => p.Nombres)
                .HasColumnName("NOMBRES");

            builder.Property(p => p.Apellidos)
                .HasColumnName("APELLIDOS");

            builder.Property(p => p.Correo)
                .HasColumnName("CORREO");
        }
    }
}
