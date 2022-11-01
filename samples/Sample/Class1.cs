using Enhanced.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Sample;

[ContainerEntry(ServiceLifetime.Singleton, typeof(Class1))]
public class Class1
{
}