using Enhanced.DependencyInjection.CodeGeneration.Registrations;

namespace Enhanced.DependencyInjection.CodeGeneration;

[Generator(LanguageNames.CSharp)]
internal partial class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if ENABLE_DEBUGGER
        System.Diagnostics.Debugger.Launch();
#endif
        var registrations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (syntaxNode, token) =>
                {
                    token.ThrowIfCancellationRequested();
                    return syntaxNode.IsClassWithAttribute();
                },
                static (syntaxContext, token) => GetRegistration(syntaxContext, token))
            .WhereNotNull()
            .Collect();

        var valueProvider = context.CompilationProvider.Combine(registrations);

        context.RegisterPostInitializationOutput(PreGenerate);
        context.RegisterImplementationSourceOutput(
            valueProvider,
            static (ctx, source) => Generate(ctx, source.Left, source.Right));
    }

    private static void PreGenerate(IncrementalGeneratorPostInitializationContext ctx)
    {
        ctx.AddSource("Extensions.Pre.g.cs", GetModulePreRegistrationSource());
    }

    private static void Generate(
        SourceProductionContext ctx,
        Compilation compilation,
        ImmutableArray<IRegistration> registrations)
    {
        var moduleContext = CreateModuleContext(ctx, compilation);
        var references = GetReferenceModules(compilation, ctx.CancellationToken);

        ctx.AddSource("Extensions.g.cs", GetModuleRegistrationSource(moduleContext.ModuleNamespace));
        ctx.AddSource("Attributes.g.cs", GetModuleAttributeSource(moduleContext.ModuleNamespace));
        ctx.AddSource("Module.g.cs", GetModuleSource(references, registrations, moduleContext));
    }

    private static IRegistration? GetRegistration(GeneratorSyntaxContext ctx, CancellationToken cancellationToken)
    {
        var model = ctx.SemanticModel;
        var classDeclaration = (ClassDeclarationSyntax)ctx.Node;

        IRegistration? result = null;

        foreach (var attributeList in classDeclaration.AttributeLists)
        foreach (var attribute in attributeList.Attributes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var attributeType = attribute.GetSymbolType(model);
            var registration = attributeType switch
            {
                { Name: TN.ContainerEntryAttributeName, IsGenericType: false }
                    => EntryRegistration.CreateFromPlainEntryAttribute(
                        attribute, classDeclaration, ctx),
                { Name: TN.ContainerEntryAttributeName, IsGenericType: true }
                    => EntryRegistration.CreateFromGenericEntryAttribute(
                        attribute, attributeType, classDeclaration, ctx),

                { Name: TN.SingletonAttributeName, IsGenericType: false }
                    => EntryRegistration.CreateFromPlainAttribute(
                        ServiceLifetime.Singleton, attribute, classDeclaration, ctx),
                { Name: TN.SingletonAttributeName, IsGenericType: true }
                    => EntryRegistration.CreateFromGenericAttribute(
                        ServiceLifetime.Singleton, attributeType, classDeclaration),

                { Name: TN.ScopedAttributeName, IsGenericType: false }
                    => EntryRegistration.CreateFromPlainAttribute(
                        ServiceLifetime.Scoped, attribute, classDeclaration, ctx),
                { Name: TN.ScopedAttributeName, IsGenericType: true }
                    => EntryRegistration.CreateFromGenericAttribute(
                        ServiceLifetime.Scoped, attributeType, classDeclaration),

                { Name: TN.TransientAttributeName, IsGenericType: false }
                    => EntryRegistration.CreateFromPlainAttribute(
                        ServiceLifetime.Transient, attribute, classDeclaration, ctx),
                { Name: TN.TransientAttributeName, IsGenericType: true }
                    => EntryRegistration.CreateFromGenericAttribute(
                        ServiceLifetime.Transient, attributeType, classDeclaration),

                { Name: TN.OptionsAttributeName }
                    => OptionsRegistration.Create(attribute, classDeclaration, ctx),

                _ => null
            };

            if (registration is null)
                continue;

            if (result is not null)
                return new ErrorRegistration(Diagnostics.ECHDI04(
                    DiagnosticSeverity.Error,
                    attribute.GetLocation(),
                    classDeclaration.Identifier.ValueText));

            result = registration;
        }

        return result;
    }

    private static ModuleContext CreateModuleContext(SourceProductionContext ctx, Compilation compilation)
    {
        var moduleNamespace
            = compilation.AssemblyName?.Replace(' ', '_')
              ?? "DynamicLib";

        return new ModuleContext(
            moduleNamespace,
            compilation,
            ctx.ReportDiagnostic,
            ctx.CancellationToken
        );
    }
}