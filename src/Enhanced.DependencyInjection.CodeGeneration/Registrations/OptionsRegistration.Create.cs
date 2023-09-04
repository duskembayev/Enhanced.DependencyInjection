namespace Enhanced.DependencyInjection.CodeGeneration.Registrations;

internal sealed partial class OptionsRegistration
{
    public static IRegistration Create(
        AttributeSyntax attribute,
        ClassDeclarationSyntax classDeclaration,
        GeneratorSyntaxContext ctx)
    {
        if (attribute.ArgumentList is null || attribute.ArgumentList.Arguments.Count <= 0)
            return new OptionsRegistration(classDeclaration, null);

        var value = attribute.ArgumentList.Arguments[0]
            .Expression
            .FindStringValue(ctx.SemanticModel);

        return new OptionsRegistration(classDeclaration, value);
    }
}