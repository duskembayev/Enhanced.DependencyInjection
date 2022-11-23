namespace Enhanced.DependencyInjection;

/// <summary>
/// Attribute to declare implementation entry lifetime and implemented services.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ContainerEntryAttribute : Attribute
{
    /// <summary>
    /// Define implementation entry lifetime and implemented service.
    /// </summary>
    /// <param name="lifetime">Specifies the lifetime of a service.</param>
    /// <param name="interface0">The Type of the service.</param>
    public ContainerEntryAttribute(ServiceLifetime lifetime, Type interface0)
        : this(lifetime, interface0, Array.Empty<Type>())
    {
    }

    /// <summary>
    /// Define implementation entry lifetime and implemented services.
    /// </summary>
    /// <param name="lifetime">Specifies the lifetime of a service.</param>
    /// <param name="interface0">The Type of the service.</param>
    /// <param name="interfaces">Additional types of the service.</param>
    public ContainerEntryAttribute(ServiceLifetime lifetime, Type interface0, params Type[] interfaces)
    {
        Lifetime = lifetime;
        Interface0 = interface0;
        Interfaces = interfaces;
    }

    internal ServiceLifetime Lifetime { get; }
    internal Type? Interface0 { get; }
    internal Type[] Interfaces { get; }
}