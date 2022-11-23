namespace Enhanced.DependencyInjection.CodeGeneration.Registrations;

internal sealed class EntryRegistration : RegistrationWithDiagnostics
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

    public override void Write(TextWriter writer, ModuleContext ctx)
    {
        WriteCore(writer);
        base.Write(writer, ctx);
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

    public static IRegistration Create(
        AttributeSyntax attribute,
        ClassDeclarationSyntax classDeclaration,
        GeneratorSyntaxContext ctx)
    {
        var model = ctx.SemanticModel;

        if (attribute.ArgumentList is not {Arguments.Count: >= 1})
            return CreateDefinitionError(attribute);

        if (!attribute.TryGetEnumValue<ServiceLifetime>(0, model, out var serviceLifetime))
            return CreateDefinitionError(attribute);

        var interfaces = attribute.ArgumentList.Arguments
            .FindTypeOfExpressions(1)
            .Select(t => model.GetTypeInfo(t).Type!)
            .ToImmutableArray();

        if (interfaces.Length != attribute.ArgumentList.Arguments.Count - 1)
            return CreateDefinitionError(attribute);

        return new EntryRegistration(serviceLifetime.Value, classDeclaration, interfaces);
    }

    private static ErrorRegistration CreateDefinitionError(AttributeSyntax attribute)
    {
        return new ErrorRegistration(Diagnostics.ECHDI03(attribute.GetLocation(), DiagnosticSeverity.Error));
    }
}