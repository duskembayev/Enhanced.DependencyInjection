namespace Enhanced.DependencyInjection.CodeGeneration;

internal static class TypeNames
{
    public const string GlobModuleInterface =
        "global::Enhanced.DependencyInjection.Modules.IContainerModule";

    public const string GlobGeneratedCodeAttribute =
        "global::System.CodeDom.Compiler.GeneratedCodeAttribute";

    public const string GlobServiceCollectionInterface =
        "global::Microsoft.Extensions.DependencyInjection.IServiceCollection";

    public const string GlobServiceLifetimeEnum =
        "global::Microsoft.Extensions.DependencyInjection.ServiceLifetime";

    public const string ModuleClass =
        "ContainerModule";

    public const string ContainerEntryAttributeName =
        "ContainerEntryAttribute";

    public const string SingletonAttributeName =
        "SingletonAttribute";

    public const string ScopedAttributeName =
        "ScopedAttribute";

    public const string TransientAttributeName =
        "TransientAttribute";

    public const string ExtensionsNamespace =
        "Enhanced.DependencyInjection.Extensions";
}