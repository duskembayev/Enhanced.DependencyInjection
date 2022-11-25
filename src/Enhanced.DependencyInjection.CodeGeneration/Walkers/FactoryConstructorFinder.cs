using Microsoft.CodeAnalysis.CSharp;

namespace Enhanced.DependencyInjection.CodeGeneration.Walkers;

internal class FactoryConstructorFinder : CSharpSyntaxWalker
{
    private readonly SemanticModel _model;
    private bool _explicitCtorFound;
    private readonly List<ConstructorDeclarationSyntax> _result = new();

    public FactoryConstructorFinder(SemanticModel model)
    {
        _model = model;
    }
    
    public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        var explicitCtor = node.AttributeLists
            .SelectMany(syntax => syntax.Attributes)
            .Any(syntax => syntax.IsTypeFullName(_model, TN.FactoryConstructorAttribute));

        if (explicitCtor && !_explicitCtorFound)
        {
            _explicitCtorFound = true;
            _result.Clear();
        }
        
        if (explicitCtor == _explicitCtorFound)
            _result.Add(node);
    }

    public IReadOnlyCollection<ConstructorDeclarationSyntax> Result => _result;
}