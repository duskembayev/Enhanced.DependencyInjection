using Enhanced.DependencyInjection.CodeGeneration.Walkers;

namespace Enhanced.DependencyInjection.CodeGeneration.Registrations;

internal sealed class EntryByFactoryRegistration : RegistrationWithDiagnostics
{
    private readonly ServiceLifetime _factoryLifetime;
    private readonly ClassDeclarationSyntax _implType;
    private readonly ImmutableArray<ITypeSymbol> _interfaces;
    private readonly ConstructorDeclarationSyntax? _ctorDeclaration;
    private readonly IReadOnlyCollection<ParameterSyntax> _arguments;

    private EntryByFactoryRegistration(
        ServiceLifetime factoryLifetime,
        ClassDeclarationSyntax implType,
        ImmutableArray<ITypeSymbol> interfaces,
        ConstructorDeclarationSyntax? ctorDeclaration,
        IReadOnlyCollection<ParameterSyntax> arguments)
    {
        _factoryLifetime = factoryLifetime;
        _implType = implType;
        _interfaces = interfaces;
        _ctorDeclaration = ctorDeclaration;
        _arguments = arguments;
    }

    public override void Write(TextWriter writer, ModuleContext ctx)
    {
        var ns = _implType.GetNamespace();
        var implTypeName = _implType.Identifier.ValueText;
        
        
        
        base.Write(writer, ctx);
    }

    public static IRegistration Create(AttributeSyntax attribute, GeneratorSyntaxContext ctx)
    {
        var model = ctx.SemanticModel;
        var classDeclaration = (ClassDeclarationSyntax) ctx.Node;

        if (attribute.ArgumentList is not {Arguments.Count: >= 1})
            return CreateDefinitionError(attribute);

        if (!attribute.TryGetEnumValue<ServiceLifetime>(0, model, out var factoryLifetime))
            return CreateDefinitionError(attribute);

        var interfaces = attribute.ArgumentList.Arguments
            .FindTypeOfExpressions(1)
            .Select(t => model.GetTypeInfo(t).Type!)
            .ToImmutableArray();

        if (interfaces.Length != attribute.ArgumentList.Arguments.Count - 1)
            return CreateDefinitionError(attribute);

        if (factoryLifetime.Value == ServiceLifetime.Transient)
            return new ErrorRegistration(Diagnostics.ECHDI04(attribute.GetLocation(), factoryLifetime.Value));

        if (!TryGetSingleOrDefaultCtor(classDeclaration, model, out var ctorDeclaration))
            return new ErrorRegistration(Diagnostics.ECHDI05(classDeclaration.GetLocation()));

        var arguments = ctorDeclaration != null
            ? GetArguments(ctorDeclaration, model)
            : Array.Empty<ParameterSyntax>();

        return new EntryByFactoryRegistration(factoryLifetime.Value, classDeclaration, interfaces, ctorDeclaration, arguments);
    }

    private static bool TryGetSingleOrDefaultCtor(
        ClassDeclarationSyntax classDeclaration,
        SemanticModel model,
        out ConstructorDeclarationSyntax? ctorDeclaration)
    {
        var finder = new FactoryConstructorFinder(model);
        classDeclaration.Accept(finder);

        if (finder.Result.Count > 1)
        {
            ctorDeclaration = null;
            return false;
        }

        ctorDeclaration = finder.Result.SingleOrDefault();
        return true;
    }

    private static IReadOnlyCollection<ParameterSyntax> GetArguments(
        ConstructorDeclarationSyntax ctorDeclaration,
        SemanticModel model)
    {
        var finder = new FactoryArgumentFinder(model);
        ctorDeclaration.Accept(finder);
        return finder.Result;
    }

    private static ErrorRegistration CreateDefinitionError(AttributeSyntax attribute)
    {
        return new ErrorRegistration(Diagnostics.ECHDI03(attribute.GetLocation()));
    }
}