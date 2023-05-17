namespace Enhanced.DependencyInjection;

[AttributeUsage(AttributeTargets.Assembly)]
public class ContainerModuleAttribute : Attribute
{
    public ContainerModuleAttribute(Type moduleType)
    {
        ModuleType = moduleType;
    }

    internal Type ModuleType { get; }
}