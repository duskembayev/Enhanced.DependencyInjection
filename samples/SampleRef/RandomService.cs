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