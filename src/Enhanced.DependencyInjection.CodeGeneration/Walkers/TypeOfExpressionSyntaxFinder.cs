using Microsoft.CodeAnalysis.CSharp;

namespace Enhanced.DependencyInjection.CodeGeneration.Walkers;

internal class TypeOfExpressionSyntaxFinder : CSharpSyntaxWalker
{
    private readonly List<TypeOfExpressionSyntax> _result = new();

    public IReadOnlyCollection<TypeOfExpressionSyntax> Result => _result;

    public override void VisitTypeOfExpression(TypeOfExpressionSyntax node) => _result.Add(node);
}