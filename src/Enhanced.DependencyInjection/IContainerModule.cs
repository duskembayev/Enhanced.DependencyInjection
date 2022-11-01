namespace Enhanced.DependencyInjection;

public interface IContainerModule
{
    void AddEntries(IServiceCollection serviceCollection);
}