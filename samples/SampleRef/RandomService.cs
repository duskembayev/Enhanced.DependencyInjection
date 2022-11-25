using Enhanced.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace SampleRef;

[ContainerEntry(ServiceLifetime.Transient, typeof(IRandomService))]
internal sealed class RandomService : IRandomService
{
    private readonly Random _random;

    public RandomService()
    {
        _random = new Random(Environment.TickCount);
    }
    
    public int GetInt()
    {
        return _random.Next();
    }
}

[ContainerEntryByFactory(ServiceLifetime.Singleton)]
internal sealed class ServiceWithArgument
{
    [FactoryConstructor]
    public ServiceWithArgument([FactoryArgument] int seed, IRandomService randomService)
    {
    }

    [global::Enhanced.DependencyInjection.FactoryConstructor]
    public ServiceWithArgument()
    {
    }
}