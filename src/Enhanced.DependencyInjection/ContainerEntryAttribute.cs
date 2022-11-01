namespace Enhanced.DependencyInjection;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ContainerEntryAttribute : Attribute
{
    public ContainerEntryAttribute(ServiceLifetime lifetime, Type interface0)
        : this(lifetime, interface0, Array.Empty<Type>())
    {
    }

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