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
}