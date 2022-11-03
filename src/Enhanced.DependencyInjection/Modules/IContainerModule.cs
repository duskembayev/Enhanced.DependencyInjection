namespace Enhanced.DependencyInjection.Modules;

public interface IContainerModule
{
    void AddEntries(IServiceCollection serviceCollection);
}