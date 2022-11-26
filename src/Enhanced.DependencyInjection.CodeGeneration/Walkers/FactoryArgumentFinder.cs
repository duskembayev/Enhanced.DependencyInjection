using Microsoft.CodeAnalysis.CSharp;

namespace Enhanced.DependencyInjection.CodeGeneration.Walkers;

internal class FactoryArgumentFinder : CSharpSyntaxWalker
{
    private readonly SemanticModel _model;
    private readonly List<ParameterSyntax> _result = new();

    public FactoryArgumentFinder(SemanticModel model)
    {
        _model = model;
    }

    public override void VisitParameter(ParameterSyntax node)
    {
        var isFactoryArg = node.AttributeLists
            .SelectMany(syntax => syntax.Attributes)
            .Any(syntax => syntax.IsTypeFullName(_model, TN.FactoryConstructorAttribute));

        if (isFactoryArg)
            _result.Add(node);
    }

    public IReadOnlyCollection<ParameterSyntax> Result => _result;
}