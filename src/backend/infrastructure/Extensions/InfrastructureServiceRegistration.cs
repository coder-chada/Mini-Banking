using ApplicationService.Accounts.Contracts;
using ApplicationService.BankTransactions.Contracts;
using ApplicationService.Common;
using ApplicationService.Common.Contracts;
using ApplicationService.Users.Contracts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
                                                                   IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("oracle");

            if (connectionString is null)
                throw new Exception();

            services.AddDbContext<MyDBContext>(opt => opt.UseOracle(connectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBankTransactionRepository, BankTransactionRepository>();
            services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();
            services.AddScoped<IDomainEventCollector, DomainEventCollector>();

            return services;
        }
    }
}
