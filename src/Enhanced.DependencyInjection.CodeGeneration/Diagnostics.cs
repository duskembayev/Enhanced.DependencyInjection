namespace Enhanced.DependencyInjection.CodeGeneration;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
internal static class Diagnostics
{
    private static readonly DiagnosticDescriptor _ECHDI01 =
        new DiagnosticDescriptor("ENHDI01", "Type resolve error", "Unable to resolve type '{0}'",
            "EnhDiModuleGenerator", defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _ECHDI05 =
        new DiagnosticDescriptor("ENHDI05", "Property resolve error", "Unable to resolve property '{0}'",
            "EnhDiModuleGenerator", defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _ECHDI03 =
        new DiagnosticDescriptor("ENHDI03", "Invalid entry definition", "The entry definition has invalid format",
            "EnhDiModuleGenerator", defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _ECHDI04 =
        new DiagnosticDescriptor("ENHDI04", "Incompatible attributes",
            "The attributes of this class cannot be used together",
            "EnhDiModuleGenerator", defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);

    internal static Diagnostic ECHDI01(string typeName, DiagnosticSeverity severity) =>
        Diagnostic.Create(_ECHDI01, Location.None, severity,
            null, null, typeName);

    internal static Diagnostic ECHDI05(string propertyName, DiagnosticSeverity severity) =>
        Diagnostic.Create(_ECHDI05, Location.None, severity,
            null, null, messageArgs: new object?[] { propertyName });

    internal static Diagnostic ECHDI03(Location location, DiagnosticSeverity severity) =>
        Diagnostic.Create(_ECHDI03, location, severity);

    internal static Diagnostic ECHDI04(Location location, DiagnosticSeverity severity) =>
        Diagnostic.Create(_ECHDI04, location, severity);
}