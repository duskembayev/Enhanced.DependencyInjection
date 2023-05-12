# Enhanced.DependencyInjection 

[![Nuget](https://img.shields.io/nuget/v/Enhanced.DependencyInjection?style=flat-square)](https://www.nuget.org/packages/Enhanced.DependencyInjection)

`Enhanced.DependencyInjection` is a NuGet package for simplified dependency registration in .NET Core DI container. It uses C# attributes and a source generator for explicit registration, making the process more understandable and faster. It makes applications more resilient and easily extensible, and welcomes community contributions.

## Motivation

`Enhanced.DependencyInjection` was created with the goal of simplifying dependency registration in .NET Core applications. By using C# attributes and a source generator, the package provides an intuitive and explicit way to register dependencies, making the process faster and easier to understand.

However, the main motivation behind `Enhanced.DependencyInjection` is to provide developers with full control over the lifecycle of dependencies and to see that lifecycle in the context of implementation code. This helps avoid separating knowledge about dependencies between different parts of the application and improves code understanding, which speeds up development and reduces the likelihood of errors.

## Features
`Enhanced.DependencyInjection` provides the following key features:

- Integration with .NET Core dependency injection.
- Support for all service lifetimes: transient, scoped, and singleton.
- Implicit registration of dependencies from sattelite modules.

## Quick start

### Install package
```shell
dotnet add package Enhanced.DependencyInjection
```

### Register dependencies
```csharp
using Enhanced.DependencyInjection;

namespace MyProject;

[ContainerEntry(ServiceLifetime.Singleton, typeof(IAwesomeService))]
internal sealed class AwesomeService : IAwesomeService {
    // your awesome code
}
```

### Add generated modules in container
```csharp
using MyProject.Enhanced.DependencyInjection;

// ... 

services.AddEnhancedModules();

// ...
```

## How it works?

The package uses a source generator, powered by Roslyn, to automatically generate registration code at design- or compile-time.

`Enhanced.DependencyInjection` generates a separate module for each assembly that uses the attributes.

```csharp

using Enhanced.DependencyInjection.Extensions;

namespace MyProject.Enhanced.DependencyInjection
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Enhanced.DependencyInjection.CodeGeneration", "1.0.1")]
    public sealed class ContainerModule : global::Enhanced.DependencyInjection.Modules.IContainerModule
    {
        public void AddEntries(global::Microsoft.Extensions.DependencyInjection.IServiceCollection sc)
        {
            sc.Entry<global::MyProject.AwesomeService>(global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton, typeof(global::MyProject.IAwesomeService));
        }
    }
}

```

In addition, an extension method is generated for each module that can be used to register the module with the DI container.

```csharp
using Enhanced.DependencyInjection.Extensions;

namespace MyProject.Enhanced.DependencyInjection {
    /// <summary>
    /// Add generated module of current assembly to <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="this">Collection of service descriptors.</param>
    /// <returns>Collection of service descriptors.</returns>
    internal static class ContainerModuleExtensions {
        public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddEnhancedModules(this global::Microsoft.Extensions.DependencyInjection.IServiceCollection @this) {
            @this.Module<ContainerModule>();
            return @this;
        }
    }
}
```

*It's important to note that using the generated extension method is typically only necessary in the entry assembly, since sattelite modules will be automatically included.*

## Conclusion
By using **Enhanced.DependencyInjection**, developers can simplify the process of dependency registration, keep their registration code clean and maintainable, and ensure that their dependencies are registered correctly with the DI container. The use of Roslyn Source Generators also ensures that the registration code is generated efficiently and without unnecessary overhead.
