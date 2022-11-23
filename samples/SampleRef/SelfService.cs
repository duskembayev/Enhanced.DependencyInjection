using Enhanced.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace SampleRef;

[ContainerEntry(ServiceLifetime.Singleton, typeof(SelfService), typeof(object))]
public class SelfService
{
    
}