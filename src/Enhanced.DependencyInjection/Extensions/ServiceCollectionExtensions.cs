﻿using System.ComponentModel;
using Enhanced.DependencyInjection.Modules;
using Microsoft.Extensions.Configuration;

namespace Enhanced.DependencyInjection.Extensions;

/// <summary>
///     Extension methods to <see cref="IServiceCollection" />.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Add service to <see cref="IServiceCollection" /> as self.
    /// </summary>
    /// <param name="serviceCollection">
    ///     Collection of service descriptors.
    /// </param>
    /// <param name="lifetime">
    ///     Specifies the lifetime of a service.
    /// </param>
    /// <typeparam name="TImpl">
    ///     The Type implementing the service.
    /// </typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Entry<TImpl>(this IServiceCollection serviceCollection, ServiceLifetime lifetime)
        where TImpl : class
    {
        serviceCollection.Add(new ServiceDescriptor(typeof(TImpl), typeof(TImpl), lifetime));
    }

    /// <summary>
    ///     Add service to <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">Collection of service descriptors.</param>
    /// <param name="lifetime">Specifies the lifetime of a service.</param>
    /// <param name="interface0">The Type of the service.</param>
    /// <typeparam name="TImpl">The Type implementing the service.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Entry<TImpl>(
        this IServiceCollection serviceCollection,
        ServiceLifetime lifetime, Type interface0)
        where TImpl : class
    {
        serviceCollection.Add(new ServiceDescriptor(interface0, typeof(TImpl), lifetime));
    }

    /// <summary>
    ///     Add service to <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">
    ///     Collection of service descriptors.
    /// </param>
    /// <param name="lifetime">
    ///     Specifies the lifetime of a service.
    /// </param>
    /// <param name="interface0">
    ///     The Type of the service.
    /// </param>
    /// <param name="interfaces">
    ///     Additional types of the service.
    /// </param>
    /// <typeparam name="TImpl">
    ///     The Type implementing the service.
    /// </typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Entry<TImpl>(
        this IServiceCollection serviceCollection,
        ServiceLifetime lifetime, Type interface0, params Type[] interfaces)
        where TImpl : class
    {
        serviceCollection.Entry<TImpl>(lifetime, interface0);

        foreach (var @interface in interfaces)
            serviceCollection.Add(new ServiceDescriptor(@interface, sp => sp.GetRequiredService(interface0), lifetime));
    }

    /// <summary>
    ///     Add configuration instance to <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">Collection of service descriptors.</param>
    /// <param name="configuration">The configuration being bound.</param>
    /// <typeparam name="TImpl">The type of options being configured.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Options<TImpl>(this IServiceCollection serviceCollection, IConfiguration? configuration)
        where TImpl : class
    {
        if (configuration is null)
        {
            serviceCollection.Configure<TImpl>(static _ => { });
            return;
        }

        serviceCollection.Configure<TImpl>(configuration);
    }

    /// <summary>
    ///     Add generated module to <see cref="IServiceCollection" /> by type.
    /// </summary>
    /// <param name="serviceCollection">
    ///     The <see cref="IServiceCollection" /> to add the services to.
    /// </param>
    /// <param name="configuration">
    ///     The configuration being bound.
    /// </param>
    /// <typeparam name="TModule">
    ///     Module type with default constructor
    /// </typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Module<TModule>(this IServiceCollection serviceCollection, IConfiguration? configuration)
        where TModule : IContainerModule, new()
    {
        var module = new TModule();
        module.AddEntries(serviceCollection, configuration);
    }
}