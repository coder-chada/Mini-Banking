using ApplicationService.Accounts.Contracts;
using ApplicationService.Accounts.Services;
using ApplicationService.BankTransactions.Contracts;
using ApplicationService.BankTransactions.Services;
using ApplicationService.Common;
using ApplicationService.Common.Contracts;
using ApplicationService.Users.Contracts;
using ApplicationService.Users.Services;
using Microsoft.Extensions.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IIdempotencyExecutor, IdempotencyExecutor>();
        services.AddScoped<IBankTransactionService, BankTransactionService>();

        return services;
    }
}