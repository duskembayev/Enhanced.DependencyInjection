namespace Enhanced.DependencyInjection.Modules;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ContainerModuleAttribute : Attribute
{
    public ContainerModuleAttribute(Type moduleType)
    {
        ModuleType = moduleType;
    }

    internal Type ModuleType { get; }
}