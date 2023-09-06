namespace Enhanced.DependencyInjection.CodeGeneration.Registrations;

internal sealed partial class OptionsRegistration : IRegistration
{
    private readonly ClassDeclarationSyntax _implType;
    private readonly string? _key;

    private OptionsRegistration(ClassDeclarationSyntax implType, string? key)
    {
        _implType = implType;
        _key = key;
    }

    public void Write(TextWriter writer, ModuleContext ctx)
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

        writer.Write("serviceCollection.Options<{0}>(",
            implSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

        if (_key is not null)
            writer.Write("configuration?.GetSection(\"{0}\")", _key);
        else
            writer.Write("configuration");

        writer.WriteLine(");");
    }
}