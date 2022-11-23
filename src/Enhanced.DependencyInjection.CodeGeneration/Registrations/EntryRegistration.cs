namespace Enhanced.DependencyInjection.CodeGeneration.Registrations;

internal sealed class EntryRegistration : IRegistration
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

    public void Write(TextWriter writer, TypeNames tn)
    {
        var serviceLifetimeEnumName = tn.ServiceLifetimeEnumName;
        var ns = _implType.GetNamespace();

        writer.Write("sc.Entry<global::{0}.{1}>(", ns, _implType.Identifier.ValueText);
        writer.Write("{0}.{1:G}", serviceLifetimeEnumName, _lifetime);

        foreach (var @interface in _interfaces)
        {
            var interfaceFullName = @interface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            writer.Write(", typeof({0})", interfaceFullName);
        }

        writer.WriteLine(");");
    }

    public static IRegistration? Resolve(
        AttributeSyntax attribute,
        ClassDeclarationSyntax classDeclaration,
        SemanticModel model)
    {
        if (attribute.ArgumentList is not {Arguments.Count: >= 1})
            return null;

        if (!attribute.TryGetEnumValue<ServiceLifetime>(0, model, out var serviceLifetime))
            return null;

        var interfaces = attribute.ArgumentList.Arguments
            .FindTypeOfExpressions(1)
            .Select(t => model.GetTypeInfo(t).Type!)
            .ToImmutableArray();

        return new EntryRegistration(serviceLifetime.Value, classDeclaration, interfaces);
    }
}