using AP.Application.Interfaces;
using AP.Persistence.Repository;

namespace AP.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IAdvPlatformRepository, AdvPlatformRepository>();

        return services;
    }
}