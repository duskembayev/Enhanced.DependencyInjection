namespace Enhanced.DependencyInjection.CodeGeneration;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
[SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeEvident")]
internal static class Diagnostics
{
    private static readonly DiagnosticDescriptor _ECHDI01 =
        new DiagnosticDescriptor("ENHDI01", "Type resolve error",
            "Unable to resolve type '{0}'",
            "EnhDiModuleGenerator", defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _ECHDI05 =
        new DiagnosticDescriptor("ENHDI05", "Property resolve error",
            "Unable to resolve property '{0}'",
            "EnhDiModuleGenerator", defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _ECHDI06 =
        new DiagnosticDescriptor("ENHDI06", "Invalid implementation",
            "Implementation type is not assignable to type '{0}'",
            "EnhDiModuleGenerator", defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _ECHDI03 =
        new DiagnosticDescriptor("ENHDI03", "Invalid entry definition",
            "The entry definition has invalid format",
            "EnhDiModuleGenerator", defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _ECHDI04 =
        new DiagnosticDescriptor("ENHDI04", "Incompatible attributes",
            "The attributes of type '{0}' cannot be used together",
            "EnhDiModuleGenerator", defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);

    internal static Diagnostic ECHDI01(DiagnosticSeverity severity, Location location, string typeName)
        => Diagnostic.Create(_ECHDI01, location, severity, null, null, typeName);

    internal static Diagnostic ECHDI05(DiagnosticSeverity severity, string propertyName)
        => Diagnostic.Create(_ECHDI05, Location.None, severity, null, null, propertyName);

    internal static Diagnostic ECHDI03(DiagnosticSeverity severity, Location location)
        => Diagnostic.Create(_ECHDI03, location, severity);

    internal static Diagnostic ECHDI04(DiagnosticSeverity severity, Location location, string typeName)
        => Diagnostic.Create(_ECHDI04, location, severity, null, null, typeName);

    internal static Diagnostic ECHDI06(DiagnosticSeverity severity, Location location, string interfaceName)
        => Diagnostic.Create(_ECHDI06, location, severity, null, null, interfaceName);
}