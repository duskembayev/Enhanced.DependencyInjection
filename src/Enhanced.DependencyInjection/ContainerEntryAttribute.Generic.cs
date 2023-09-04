#if LANG11_OR_GREATER

namespace Enhanced.DependencyInjection;

/// <summary>
///     Represents an attribute that is used to declare implementation entry lifetime and implemented services.
/// </summary>
/// <typeparam name="TInterface">
///     The Type of the service.
/// </typeparam>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ContainerEntryAttribute<TInterface> : Attribute
{
    /// <summary>
    ///     Define implementation entry lifetime.
    /// </summary>
    /// <param name="lifetime">
    ///     Specifies the lifetime of a service.
    /// </param>
    public ContainerEntryAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }

    internal ServiceLifetime Lifetime { get; }
}

/// <summary>
///     Represents an attribute that is used to declare implementation entry lifetime and implemented services.
/// </summary>
/// <typeparam name="TInterface1">
///     First type of the service.
/// </typeparam>
/// <typeparam name="TInterface2">
///     Second type of the service.
/// </typeparam>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ContainerEntryAttribute<TInterface1, TInterface2> : Attribute
{
    /// <summary>
    ///     Define implementation entry lifetime.
    /// </summary>
    /// <param name="lifetime">
    ///     Specifies the lifetime of a service.
    /// </param>
    public ContainerEntryAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }

    internal ServiceLifetime Lifetime { get; }
}

/// <summary>
///     Represents an attribute that is used to declare implementation entry lifetime and implemented services.
/// </summary>
/// <typeparam name="TInterface1">
///     First type of the service.
/// </typeparam>
/// <typeparam name="TInterface2">
///     Second type of the service.
/// </typeparam>
/// <typeparam name="TInterface3">
///     Third type of the service.
/// </typeparam>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ContainerEntryAttribute<TInterface1, TInterface2, TInterface3> : Attribute
{
    /// <summary>
    ///     Define implementation entry lifetime.
    /// </summary>
    /// <param name="lifetime">
    ///     Specifies the lifetime of a service.
    /// </param>
    public ContainerEntryAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }

    internal ServiceLifetime Lifetime { get; }
}

#endif