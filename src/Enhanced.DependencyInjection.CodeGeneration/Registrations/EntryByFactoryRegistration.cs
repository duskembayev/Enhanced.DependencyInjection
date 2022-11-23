namespace Enhanced.DependencyInjection.CodeGeneration.Registrations;

internal sealed class EntryByFactoryRegistration : RegistrationWithDiagnostics
{
    public override void Write(TextWriter writer, ModuleContext ctx)
    {
        base.Write(writer, ctx);
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
        
        if (serviceLifetime == ServiceLifetime.Scoped)
        {
            
        }
    }
    
    private static ErrorRegistration CreateDefinitionError(AttributeSyntax attribute)
    {
        return new ErrorRegistration(Diagnostics.ECHDI03(attribute.GetLocation(), DiagnosticSeverity.Error));
    }
}