using System.ComponentModel;
using System.Reflection;
using Enhanced.DependencyInjection.Modules;

namespace Enhanced.DependencyInjection.Extensions;

/// <summary>
/// Extension methods to <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add service to <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="this">Collection of service descriptors.</param>
    /// <param name="lifetime">Specifies the lifetime of a service.</param>
    /// <param name="interface0">The Type of the service.</param>
    /// <typeparam name="TImpl">The Type implementing the service.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Entry<TImpl>(this IServiceCollection @this,
        ServiceLifetime lifetime, Type interface0)
        where TImpl : class
    {
        @this.Add(new ServiceDescriptor(interface0, typeof(TImpl), lifetime));
    }

    /// <summary>
    /// Add service to <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="this">Collection of service descriptors.</param>
    /// <param name="lifetime">Specifies the lifetime of a service.</param>
    /// <param name="interface0">The Type of the service.</param>
    /// <param name="interfaces">Additional types of the service.</param>
    /// <typeparam name="TImpl">The Type implementing the service.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Entry<TImpl>(this IServiceCollection @this,
        ServiceLifetime lifetime, Type interface0, params Type[] interfaces)
        where TImpl : class
    {
        @this.Entry<TImpl>(lifetime, interface0);

        foreach (var @interface in interfaces)
            @this.Add(new ServiceDescriptor(@interface, sp => sp.GetRequiredService(interface0), lifetime));
    }

    /// <summary>
    /// Add generated module to <see cref="IServiceCollection" /> by type.
    /// </summary>
    /// <param name="this">Collection of service descriptors.</param>
    /// <typeparam name="TModule">Module type with default constructor</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Module<TModule>(this IServiceCollection @this)
        where TModule : IContainerModule, new()
    {
        var module = new TModule();
        module.AddEntries(@this);
    }

    /// <summary>
    /// Add generated module of current assembly to <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="this">Collection of service descriptors.</param>
    /// <returns>Collection of service descriptors.</returns>
    /// <exception cref="ApplicationException">Throws exception when current assembly module could not be resolved.</exception>
    public static IServiceCollection AddEnhancedModules(this IServiceCollection @this)
    {
        var callingAssembly = Assembly.GetCallingAssembly();
        var callingAssemblyName = callingAssembly
            .GetName()
            .Name;

        if (callingAssemblyName is null)
            throw new InvalidOperationException();

        var callingAssemblyModuleName = $"{callingAssemblyName}.Enhanced.DependencyInjection.ContainerModule";
        var moduleType = callingAssembly.GetType(callingAssemblyModuleName);

        if (moduleType is null || Activator.CreateInstance(moduleType) is not IContainerModule module)
            throw new ApplicationException(
                $"Container module not found. Expected type: \"{callingAssemblyModuleName}\"");

        module.AddEntries(@this);
        return @this;
    }
}