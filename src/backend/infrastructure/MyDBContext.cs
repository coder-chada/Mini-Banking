using Microsoft.EntityFrameworkCore;
using Infrastructure.PersistenceModels;
using Infrastructure.Mappers;

namespace Infrastructure
{
    internal class MyDBContext : DbContext
    {
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<AccountEntity> Accounts => Set<AccountEntity>();

        public MyDBContext(DbContextOptions<MyDBContext> opt) : base(opt)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserMap).Assembly);
        }
    }
}
