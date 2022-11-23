namespace Enhanced.DependencyInjection;

/// <summary>
///     Attribute to declare transient entry that available by factory.
/// </summary>
public sealed class ContainerEntryByFactoryAttribute : Attribute
{
    public ContainerEntryByFactoryAttribute(ServiceLifetime factoryLifetime)
    {
        FactoryLifetime = factoryLifetime;
    }

    public ServiceLifetime FactoryLifetime { get; }
}