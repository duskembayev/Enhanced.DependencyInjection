namespace Enhanced.DependencyInjection;

/// <summary>
/// Defines a module that can be used to register services with the <see cref="IServiceCollection" />
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class ContainerModuleAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerModuleAttribute" /> class.
    /// </summary>
    /// <param name="moduleType">
    /// The type of the module to register with the <see cref="IServiceCollection" />.
    /// </param>
    public ContainerModuleAttribute(Type moduleType)
    {
        ModuleType = moduleType;
    }

    internal Type ModuleType { get; }
}