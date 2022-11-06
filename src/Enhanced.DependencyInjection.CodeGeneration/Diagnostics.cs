namespace Enhanced.DependencyInjection.CodeGeneration;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public static class Diagnostics
{
    private static readonly DiagnosticDescriptor _ECHDI01 =
        new("ENHDI01", "Type resolve error", "Unable to resolve type '{0}'",
            "EnhDiModuleGenerator", defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);

    public static Diagnostic ECHDI01(string typeName, DiagnosticSeverity severity) =>
        Diagnostic.Create(_ECHDI01, Location.None, severity, null, null, typeName);
}