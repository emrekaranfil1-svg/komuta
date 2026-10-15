namespace Komuta.Api.Abstractions;

public interface IModule
{
    void RegisterServices(IServiceCollection services);
}
