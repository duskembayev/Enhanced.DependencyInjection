namespace Enhanced.DependencyInjection.CodeGeneration;

internal sealed class TypeNames
{
    private const string ModuleInterface = "Enhanced.DependencyInjection.Modules.IContainerModule";
    

    private const string GeneratedCodeAttribute = "System.CodeDom.Compiler.GeneratedCodeAttribute";

    private const string ServiceCollectionInterface = "Microsoft.Extensions.DependencyInjection.IServiceCollection";
    private const string ServiceLifetimeEnum = "Microsoft.Extensions.DependencyInjection.ServiceLifetime";

    public TypeNames(Compilation compilation, Action<Diagnostic> diagnosticHandler)
    {
        GeneratedCodeAttributeName = compilation
                                         .GetTypeByMetadataName(GeneratedCodeAttribute)
                                         ?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                                     ?? string.Empty;

        if (string.IsNullOrEmpty(GeneratedCodeAttribute))
            diagnosticHandler(Diagnostics.ECHDI01(GeneratedCodeAttribute, DiagnosticSeverity.Error));

        ModuleInterfaceName = compilation
                                  .GetTypeByMetadataName(ModuleInterface)
                                  ?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                              ?? string.Empty;

        if (string.IsNullOrEmpty(ModuleInterfaceName))
            diagnosticHandler(Diagnostics.ECHDI01(ModuleInterface, DiagnosticSeverity.Error));

        ServiceCollectionName = compilation
                                    .GetTypeByMetadataName(ServiceCollectionInterface)
                                    ?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                                ?? string.Empty;

        if (string.IsNullOrEmpty(ServiceCollectionName))
            diagnosticHandler(Diagnostics.ECHDI01(ServiceCollectionInterface, DiagnosticSeverity.Error));

        ServiceLifetimeEnumName = compilation
                                      .GetTypeByMetadataName(ServiceLifetimeEnum)
                                      ?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                                  ?? string.Empty;

        if (string.IsNullOrEmpty(ServiceLifetimeEnumName))
            diagnosticHandler(Diagnostics.ECHDI01(ServiceLifetimeEnum, DiagnosticSeverity.Error));
    }

    public string GeneratedCodeAttributeName { get; }
    public string ServiceLifetimeEnumName { get; }
    public string ServiceCollectionName { get; }
    public string ModuleInterfaceName { get; }
}