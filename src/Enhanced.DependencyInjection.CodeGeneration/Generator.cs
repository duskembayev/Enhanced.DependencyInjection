using Enhanced.DependencyInjection.CodeGeneration.Registrations;
using Microsoft.CodeAnalysis.Diagnostics;

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

        var valueProvider = context.AnalyzerConfigOptionsProvider
            .Combine(context.CompilationProvider)
            .Combine(registrations);

        context.RegisterSourceOutput(
            valueProvider,
            static (ctx, source) => Generate(ctx, source.Left.Left, source.Left.Right, source.Right));
    }

    private static void Generate(
        SourceProductionContext ctx,
        AnalyzerConfigOptionsProvider options,
        Compilation compilation,
        ImmutableArray<IRegistration> registrations)
    {
        var moduleContext = CreateModuleContext(ctx, compilation, options);
        var references = GetReferenceModules(compilation, ctx.CancellationToken);

        ctx.AddSource("Module.g.cs", GetModuleSource(references, registrations, moduleContext));
        ctx.AddSource("Attributes.g.cs", GetModuleAttributeSource(moduleContext));

        if (moduleContext.MethodName is not null)
            ctx.AddSource("Extensions.g.cs", GetModuleRegistrationSource(moduleContext));
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

    private static ModuleContext CreateModuleContext(
        SourceProductionContext ctx,
        Compilation compilation,
        AnalyzerConfigOptionsProvider options)
    {
        var ns = options.GetModuleNamespace(ctx);
        var moduleName = options.GetModuleName(ctx);
        var methodName = options.GetModuleRegistrationMethodName(ctx);

        return new ModuleContext(
            ns,
            moduleName,
            methodName,
            compilation,
            ctx.ReportDiagnostic,
            ctx.CancellationToken
        );
    }
}