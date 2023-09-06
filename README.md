# Enhanced.DependencyInjection 

[![license](https://flat.badgen.net/github/license/duskembayev/Enhanced.DependencyInjection)](LICENSE)
[![nuget](https://flat.badgen.net/nuget/v/Enhanced.DependencyInjection?icon=nuget)](https://www.nuget.org/packages/Enhanced.DependencyInjection)

`Enhanced.DependencyInjection` is a NuGet package for simplified dependency registration in .NET Core DI container. It uses C# attributes and a source generator for explicit registration, making the process more understandable and faster. It makes applications more resilient and easily extensible, and welcomes community contributions.

## Motivation

`Enhanced.DependencyInjection` was created with the goal of simplifying dependency registration in .NET Core applications. By using C# attributes and a source generator, the package provides an intuitive and explicit way to register dependencies, making the process faster and easier to understand.

However, the main motivation behind `Enhanced.DependencyInjection` is to provide developers with full control over the lifecycle of dependencies and to see that lifecycle in the context of implementation code. This helps avoid separating knowledge about dependencies between different parts of the application and improves code understanding, which speeds up development and reduces the likelihood of errors.

## Features
`Enhanced.DependencyInjection` provides the following key features:

- Integration with .NET Core dependency injection.
- Support for all service lifetimes: transient, scoped, and singleton.
- Support for option registration and linking it with a configuration section.
- Implicit registration of dependencies from satellite modules.

## Quick start

### Install package
```shell
dotnet add package Enhanced.DependencyInjection
```

### Mark your classes with attributes
```csharp
using Enhanced.DependencyInjection;

namespace MyProject;

[Singleton<IAwesomeService>]
internal sealed class AwesomeService : IAwesomeService {
    // your awesome code
}

[Options("Features:AwesomeService")]
internal sealed class AwesomeServiceOptions {
    // your awesome options
}

[Transient]
internal class AwesomeCalculator {
    // your awesome calculator
}
```

### Add generated modules in container
```csharp
using MyProject.Enhanced.DependencyInjection;

// ... 

services.AddEnhancedModules(configuration);

// ...
```

## How it works?

The package uses a source generator, powered by Roslyn, to automatically generate registration code at design- or compile-time.

`Enhanced.DependencyInjection` generates a separate module for each assembly that uses the attributes.

```csharp
// <auto-generated />
#nullable enable
using global::Enhanced.DependencyInjection.Extensions;
using IContainerModule = global::Enhanced.DependencyInjection.Modules.IContainerModule;
using IConfiguration = global::Microsoft.Extensions.Configuration.IConfiguration;
using IServiceCollection = global::Microsoft.Extensions.DependencyInjection.IServiceCollection;
using ServiceLifetime = global::Microsoft.Extensions.DependencyInjection.ServiceLifetime;

namespace MyProject
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Enhanced.DependencyInjection.CodeGeneration", "1.3.0")]
    public sealed class ContainerModule : global::Enhanced.DependencyInjection.Modules.IContainerModule
    {
        public void AddEntries(IServiceCollection serviceCollection, IConfiguration? configuration)
        {
            serviceCollection.Entry<global::MyProject.AwesomeService>(global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton, typeof(global::MyProject.IAwesomeService));
            serviceCollection.Entry<global::MyProject.AwesomeCalculator>(global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient);
            serviceCollection.Options<global::MyProject.AwesomeServiceOptions>(configuration.GetSection("Features:AwesomeService"));
        }
    }
}

```

In addition, an extension method is generated for each module that can be used to register the module with the DI container.

```csharp
// <auto-generated />
#nullable enable
using global::Enhanced.DependencyInjection.Extensions;
using IConfiguration = global::Microsoft.Extensions.Configuration.IConfiguration;
using IServiceCollection = global::Microsoft.Extensions.DependencyInjection.IServiceCollection;

namespace Microsoft.Extensions.DependencyInjection
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Enhanced.DependencyInjection.CodeGeneration", "1.3.0")]
    internal static class EnhancedDependencyInjectionExtensions
    {
        public static IServiceCollection AddEnhancedModules(this IServiceCollection serviceCollection, IConfiguration? configuration)
        {
            serviceCollection.Module<global::MyProject.ContainerModule>(configuration);
            return serviceCollection;
        }
    }
}
```

*It's important to note that using the generated extension method is typically only necessary in the entry assembly, since satellite modules will be automatically included.*

## Conclusion
By using **Enhanced.DependencyInjection**, developers can simplify the process of dependency registration, keep their registration code clean and maintainable, and ensure that their dependencies are registered correctly with the DI container. The use of Roslyn Source Generators also ensures that the registration code is generated efficiently and without unnecessary overhead.
