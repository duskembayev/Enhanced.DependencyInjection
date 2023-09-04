using Enhanced.DependencyInjection.CodeGeneration.Walkers;

namespace Enhanced.DependencyInjection.CodeGeneration.Extensions;

internal static class SyntaxNodeExtensions
{
    internal static INamedTypeSymbol? GetSymbolType(this SyntaxNode @this, SemanticModel semanticModel)
    {
        var symbolInfo = semanticModel.GetSymbolInfo(@this);
        var symbol = symbolInfo.Symbol;
        return symbol?.ContainingType;
    }

    internal static bool IsClassWithAttribute(this SyntaxNode @this)
    {
        return @this is ClassDeclarationSyntax {AttributeLists.Count: > 0};
    }

    internal static bool TryGetEnumValue<TEnum>(
        this AttributeSyntax @this,
        int index,
        SemanticModel semanticModel,
        [NotNullWhen(true)] out TEnum? value)
        where TEnum : struct
    {
        value = null;

        if (@this.ArgumentList is null)
            return false;

        if (@this.ArgumentList.Arguments.Count <= index)
            return false;

        var constantValue = semanticModel.GetConstantValue(@this.ArgumentList.Arguments[index].Expression);

        if (!constantValue.HasValue || constantValue.Value is null)
            return false;

        if (!Enum.IsDefined(typeof(TEnum), constantValue.Value))
            return false;

        value = (TEnum) constantValue.Value;
        return true;
    }

    internal static ImmutableArray<TypeSyntax> FindTypeOfExpressions(
        this IReadOnlyList<AttributeArgumentSyntax> @this,
        int startIndex)
    {
        var typeOfExpressionSyntaxFinder = new TypeOfExpressionSyntaxFinder();

        for (var i = startIndex; i < @this.Count; i++)
            @this[i].Expression.Accept(typeOfExpressionSyntaxFinder);

        return typeOfExpressionSyntaxFinder.Result
            .Select(syntax => syntax.Type)
            .ToImmutableArray();
    }

    internal static string? FindStringValue(this ExpressionSyntax @this, SemanticModel semanticModel)
    {
        return semanticModel.GetConstantValue(@this).Value as string;
    }

    // determine the namespace the class/enum/struct is declared in, if any
    internal static string GetNamespace(this BaseTypeDeclarationSyntax syntax)
    {
        // If we don't have a namespace at all we'll return an empty string
        // This accounts for the "default namespace" case
        var @namespace = string.Empty;

        // Get the containing syntax node for the type declaration
        // (could be a nested type, for example)
        var potentialNamespaceParent = syntax.Parent;

        // Keep moving "out" of nested classes etc until we get to a namespace
        // or until we run out of parents
        while (potentialNamespaceParent != null &&
               potentialNamespaceParent is not NamespaceDeclarationSyntax
               && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        // Build up the final namespace by looping until we no longer have a namespace declaration
        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            // We have a namespace. Use that as the type
            @namespace = namespaceParent.Name.ToString();

            // Keep moving "out" of the namespace declarations until we 
            // run out of nested namespace declarations
            while (true)
            {
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                    break;

                // Add the outer namespace as a prefix to the final namespace
                @namespace = $"{namespaceParent.Name}.{@namespace}";
                namespaceParent = parent;
            }
        }

        // return the final namespace
        return @namespace;
    }
}