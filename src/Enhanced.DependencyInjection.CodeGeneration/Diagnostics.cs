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
    private static readonly DiagnosticDescriptor _ECHDI03 =
        new("ENHDI03", "Invalid entry definition", "The entry definition has invalid format",
            "EnhDiModuleGenerator", defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);
    private static readonly DiagnosticDescriptor _ECHDI04 =
        new("ENHDI04", "Invalid lifetime", "The lifetime '{0}' is not supported by target pattern",
            "EnhDiModuleGenerator", defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);
    private static readonly DiagnosticDescriptor _ECHDI05 =
        new("ENHDI05", "Ambigious factory constructor", "There are two or more constructor methods. Use [FactoryConstructor] method to define proper one",
            "EnhDiModuleGenerator", defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);

    public static Diagnostic ECHDI01(string typeName) =>
        Diagnostic.Create(_ECHDI01, Location.None, typeName);
    public static Diagnostic ECHDI02() =>
        Diagnostic.Create(_ECHDI02, Location.None);
    public static Diagnostic ECHDI03(Location location) =>
        Diagnostic.Create(_ECHDI03, location);
    public static Diagnostic ECHDI04(Location location, ServiceLifetime lifetime) =>
        Diagnostic.Create(_ECHDI04, location, lifetime);
    public static Diagnostic ECHDI05(Location location) =>
        Diagnostic.Create(_ECHDI05, location);
}