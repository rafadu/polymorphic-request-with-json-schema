using Core.Ports.In;
using Core.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class CoreServiceExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IInterpretPayloadUseCase, InterpretPayloadUseCase>();
        return services;
    }
}
