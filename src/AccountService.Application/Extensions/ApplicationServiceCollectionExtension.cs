using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Application.Extensions;

public static class ApplicationServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceCollectionExtension).Assembly));
        return services;
    }
}
