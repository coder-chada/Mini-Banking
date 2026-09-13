using ApplicationService.Accounts.Contracts;
using ApplicationService.BankTransactions.Contracts;
using ApplicationService.Common;
using ApplicationService.Common.Contracts;
using ApplicationService.Users.Contracts;
using Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var connectionString = configuration.GetConnectionString("sqlite");

            if (connectionString is null)
                throw new KeyNotFoundException("failed to find the connection string sqlite");

            CreateTables(connectionString);

            services.AddDbContext<MyDBContext>(opt => opt.UseSqlite(connectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBankTransactionRepository, BankTransactionRepository>();
            services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();
            services.AddScoped<IDomainEventCollector, DomainEventCollector>();

            return services;
        }

        private static void CreateTables(string connectionString)
        {
            using var connection = new SqliteConnection(connectionString);

            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = """
                CREATE TABLE IF NOT EXISTS usuarios (
                    usuario_id   INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT
                    , dni        TEXT UNIQUE NOT NULL
                    , nombres    TEXT NOT NULL
                    , apellidos  TEXT NOT NULL
                    , correo     TEXT NOT NULL
                );
            """;
            command.ExecuteNonQuery();

            command.CommandText = """
                CREATE TABLE IF NOT EXISTS accounts (
                    account_id      INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT
                    , numero_cuenta TEXT UNIQUE
                    , tipo_cuenta   INTEGER
                    , tipo_currency INTEGER
                    , usuario_fk    INTEGER REFERENCES usuarios(usuario_id)
                    , balance       REAL
                );
            """;
            command.ExecuteNonQuery();

            command.CommandText = """
                CREATE TABLE IF NOT EXISTS idempotency_keys (
                    idempotency_key   TEXT NOT NULL PRIMARY KEY
                    , request_hash    TEXT NOT NULL
                    , status          INTEGER NOT NULL
                    , response_body   TEXT NULL
                    , status_code     INTEGER NULL
                    , created_at      DATE NOT NULL
                    , completed_at    DATE NULL
                    , expires_at      DATE NULL
                );
            """;
            command.ExecuteNonQuery();

            command.CommandText = """
                CREATE TABLE IF NOT EXISTS transactions (
                    transaction_id       TEXT PRIMARY KEY
                    , sender_fk          INTEGER REFERENCES accounts (account_id)
                    , receiver_fk        INTEGER REFERENCES accounts (account_id)
                    , amount             REAL
                    , created_at         DATE
                    , transaction_type   INTEGER
                    , transaction_status INTEGER
                    , row_version        INTEGER DEFAULT 0
                );
            """;
            command.ExecuteNonQuery();
        }
    }
}
