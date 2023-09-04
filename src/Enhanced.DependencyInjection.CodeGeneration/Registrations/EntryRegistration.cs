using System.Diagnostics;

namespace Enhanced.DependencyInjection.CodeGeneration.Registrations;

internal sealed partial class EntryRegistration : IRegistration
{
    private readonly ServiceLifetime _lifetime;
    private readonly ClassDeclarationSyntax _implType;
    private readonly ImmutableArray<ITypeSymbol> _interfaces;

    private EntryRegistration(
        ServiceLifetime lifetime,
        ClassDeclarationSyntax implType,
        ImmutableArray<ITypeSymbol> interfaces)
    {
        _lifetime = lifetime;
        _implType = implType;
        _interfaces = interfaces;
    }

    void IRegistration.Write(TextWriter writer, ModuleContext ctx)
    {
        var implName = $"{_implType.GetNamespace()}.{_implType.Identifier.ValueText}";
        var implSymbol = ctx.Compilation.GetTypeByMetadataName(implName);

        if (implSymbol is null)
        {
            ctx.Report(Diagnostics.ECHDI01(
                DiagnosticSeverity.Error,
                _implType.GetLocation(),
                implName));

            return;
        }

        WriteDiagnostics(implSymbol, ctx);

        writer.Write("serviceCollection.Entry<{0}>(",
            implSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        writer.Write("ServiceLifetime.{0:G}", _lifetime);

        foreach (var @interface in _interfaces)
            writer.Write(", typeof({0})", @interface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

        writer.WriteLine(");");
    }

    private void WriteDiagnostics(INamedTypeSymbol implSymbol, ModuleContext ctx)
    {
        if (_interfaces.Length <= 0)
            return;

        foreach (var @interface in _interfaces)
        {
            if (implSymbol.AllInterfaces.Contains(@interface, SymbolEqualityComparer.Default))
                continue;

            if (implSymbol.Equals(@interface, SymbolEqualityComparer.Default))
                continue;

            if (IsInheritsFrom(implSymbol, @interface))
                continue;

            ctx.Report(Diagnostics.ECHDI06(
                DiagnosticSeverity.Error,
                _implType.GetLocation(),
                @interface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
        }
    }

    private static bool IsInheritsFrom(INamedTypeSymbol implSymbol, ISymbol type)
    {
        var baseType = implSymbol.BaseType;

        while (baseType is not null)
        {
            if (baseType.Equals(type, SymbolEqualityComparer.Default))
                return true;

            baseType = baseType.BaseType;
        }

        return false;
    }
}