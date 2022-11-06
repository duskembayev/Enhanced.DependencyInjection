# Enhanced.DependencyInjection
Provides the way to register services in `IServiceCollection` using attributes and code generators

## How to use
1. Reference nuget package `Enhanced.DependencyInjection`.
2. Mark services using attribute:
```
[ContainerEntry(ServiceLifetime.Singleton, typeof(ISampleService), typeof(IHostedService))]
internal sealed class SampleService : ISampleService, IHostedService, IDisposable {
    // ...
}
```
2. Add code below to your `Startup.cs`.
```
public void ConfigureServices(IServiceCollection builder) {
    // ...
    builder.AddEnhancedModules();
    // ...
}
```