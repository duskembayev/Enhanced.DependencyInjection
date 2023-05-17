namespace Enhanced.DependencyInjection.CodeGeneration;

partial class Generator
{
    private static ImmutableArray<INamedTypeSymbol> GetReferenceModules(
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        var result = ImmutableArray.CreateBuilder<INamedTypeSymbol>();

        foreach (var reference in compilation.SourceModule.ReferencedAssemblySymbols)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!TryGetReferenceModule(reference, out var typeSymbol))
                continue;

            result.Add(typeSymbol);
        }

        return result.ToImmutable();
    }

    private static bool TryGetReferenceModule(
        IAssemblySymbol referenceSymbol,
        [NotNullWhen(true)] out INamedTypeSymbol? moduleSymbol)
    {
        var attribute = referenceSymbol
            .GetAttributes()
            .FirstOrDefault(data => data.AttributeClass?.Name == TN.ContainerModuleAttributeName);

        if (attribute is not { ConstructorArguments.Length: 1 })
        {
            moduleSymbol = null;
            return false;
        }

        if (attribute.ConstructorArguments[0] is not { Kind: TypedConstantKind.Type }
            || attribute.ConstructorArguments[0].Value is not INamedTypeSymbol symbol)
        {
            moduleSymbol = null;
            return false;
        }

        moduleSymbol = symbol;
        return true;
    }
}