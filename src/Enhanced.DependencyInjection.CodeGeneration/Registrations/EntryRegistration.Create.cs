namespace Enhanced.DependencyInjection.CodeGeneration.Registrations;

internal sealed partial class EntryRegistration
{
    internal static IRegistration CreateFromPlainEntryAttribute(
        AttributeSyntax attribute,
        ClassDeclarationSyntax classDeclaration,
        GeneratorSyntaxContext ctx)
    {
        if (attribute.ArgumentList is not {Arguments.Count: >= 1})
            return Error(attribute);

        if (!attribute.TryGetEnumValue<ServiceLifetime>(0, ctx.SemanticModel, out var lifetime))
            return Error(attribute);

        return FromPlainAttributeCore(lifetime.Value, ctx.SemanticModel, classDeclaration, attribute, 1);
    }

    internal static IRegistration CreateFromGenericEntryAttribute(
        AttributeSyntax attribute,
        INamedTypeSymbol attributeType,
        ClassDeclarationSyntax classDeclaration,
        GeneratorSyntaxContext ctx)
    {
        if (attribute.ArgumentList is not {Arguments.Count: 1})
            return Error(attribute);

        if (!attribute.TryGetEnumValue<ServiceLifetime>(0, ctx.SemanticModel, out var lifetime))
            return Error(attribute);

        return FromGenericAttributeCore(lifetime.Value, classDeclaration, attributeType);
    }

    internal static IRegistration CreateFromPlainAttribute(
        ServiceLifetime lifetime,
        AttributeSyntax attribute,
        ClassDeclarationSyntax classDeclaration,
        GeneratorSyntaxContext ctx)
    {
        return FromPlainAttributeCore(lifetime, ctx.SemanticModel, classDeclaration, attribute, 0);
    }

    internal static IRegistration CreateFromGenericAttribute(
        ServiceLifetime lifetime,
        INamedTypeSymbol attributeType,
        ClassDeclarationSyntax classDeclaration)
    {
        return FromGenericAttributeCore(lifetime, classDeclaration, attributeType);
    }

    private static IRegistration FromGenericAttributeCore(
        ServiceLifetime lifetime,
        ClassDeclarationSyntax classDeclaration,
        INamedTypeSymbol attributeType)
    {
        return new EntryRegistration(lifetime, classDeclaration, attributeType.TypeArguments);
    }

    private static IRegistration FromPlainAttributeCore(
        ServiceLifetime lifetime,
        SemanticModel semanticModel,
        ClassDeclarationSyntax classDeclaration,
        AttributeSyntax attribute,
        int startIndex)
    {
        if (attribute.ArgumentList is null || attribute.ArgumentList.Arguments.Count <= startIndex)
            return new EntryRegistration(lifetime, classDeclaration, ImmutableArray<ITypeSymbol>.Empty);

        var interfaces = attribute
            .ArgumentList
            .Arguments
            .FindTypeOfExpressions(startIndex)
            .Select(t => semanticModel.GetTypeInfo(t).Type)
            .WhereNotNull()
            .ToImmutableArray();

        if (interfaces.Length != attribute.ArgumentList.Arguments.Count - startIndex)
            return Error(attribute);

        return new EntryRegistration(lifetime, classDeclaration, interfaces);
    }

    private static ErrorRegistration Error(AttributeSyntax attribute)
    {
        return new ErrorRegistration(Diagnostics.ECHDI03(DiagnosticSeverity.Error, attribute.GetLocation()));
    }
}