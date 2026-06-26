using Infrastructure.Mappers;
using Infrastructure.PersistenceModels;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class MyDBContext : DbContext
    {
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<AccountEntity> Accounts => Set<AccountEntity>();
        public DbSet<BankTransactionEntity> BankTransactions => Set<BankTransactionEntity>();
        public DbSet<IdempotencyEntity> Idempotency => Set<IdempotencyEntity>();


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
