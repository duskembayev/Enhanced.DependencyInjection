namespace Enhanced.DependencyInjection.CodeGeneration;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public static class Diagnostics
{
    private static readonly DiagnosticDescriptor _ECHDI01 =
        new("ENHDI01", "Type resolve error", "Unable to resolve type '{0}'",
            "EnhDiModuleGenerator", defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);
    private static readonly DiagnosticDescriptor _ECHDI02 =
        new("ENHDI02", "Root namespace resolve error", "Unable to resolve root namespace",
            "EnhDiModuleGenerator", defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);

    public static Diagnostic ECHDI01(string typeName, DiagnosticSeverity severity) =>
        Diagnostic.Create(_ECHDI01, Location.None, severity, null, null, typeName);
    public static Diagnostic ECHDI02(DiagnosticSeverity severity) =>
        Diagnostic.Create(_ECHDI02, Location.None, severity, null, null);
}