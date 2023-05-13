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

    public void Write(TextWriter writer, ModuleContext ctx)
    {
        WriteCore(writer);
    }

    private void WriteCore(TextWriter writer)
    {
        var ns = _implType.GetNamespace();

        writer.Write("sc.Entry<global::{0}.{1}>(", ns, _implType.Identifier.ValueText);
        writer.Write("{0}.{1:G}", TN.GlobServiceLifetimeEnum, _lifetime);

        foreach (var @interface in _interfaces)
        {
            var interfaceFullName = @interface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            writer.Write(", typeof({0})", interfaceFullName);
        }

        writer.WriteLine(");");
    }

}