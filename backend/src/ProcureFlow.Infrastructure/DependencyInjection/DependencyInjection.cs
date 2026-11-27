using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcureFlow.Infrastructure.Persistence;

namespace ProcureFlow.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "The database connection string 'DefaultConnection' was not found.");

        services.AddDbContext<ProcureFlowDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
