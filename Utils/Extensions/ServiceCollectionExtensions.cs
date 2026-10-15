using Komuta.Api.Abstractions;
using Komuta.Api.Utils.IoC;

namespace Komuta.Api.Utils.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDependencyResolvers(this IServiceCollection services, IModule[] modules)
    {
        foreach (var module in modules)
            module.RegisterServices(services);
        return ServiceTool.Create(services);
    }
}
