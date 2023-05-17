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
        var ns = _implType.GetNamespace();

        writer.Write("serviceCollection.Entry<{0}.{1}>(", ns, _implType.Identifier.ValueText);
        writer.Write("ServiceLifetime.{0:G}", _lifetime);

        foreach (var @interface in _interfaces)
            writer.Write(", typeof({0})", @interface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

        writer.WriteLine(");");
    }
}