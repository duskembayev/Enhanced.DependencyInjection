﻿using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using Enhanced.DependencyInjection.CodeGeneration.Registrations;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Enhanced.DependencyInjection.CodeGeneration;

[Generator(LanguageNames.CSharp)]
public class ContainerEntryAttributeProcessor : IIncrementalGenerator
{
    private static readonly string ToolName;
    private static readonly string ToolVersion;

    static ContainerEntryAttributeProcessor()
    {
        var assemblyName = Assembly
            .GetExecutingAssembly()
            .GetName();

        ToolName = assemblyName.Name;
        ToolVersion = assemblyName.Version.ToString(3);
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if ENABLE_DEBUGGER
        Debugger.Launch();
#endif

        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (n, t) => n.IsClassWithAttribute(t),
                static (syntaxContext, token) => GetRegistration(syntaxContext, token))
            .WhereNotNull()
            .Collect();

        var valueProvider = context.AnalyzerConfigOptionsProvider
            .Combine(context.CompilationProvider)
            .Combine(classDeclarations);

        context.RegisterSourceOutput(
            valueProvider,
            static (ctx, source) => Generate(ctx, source.Left.Left, source.Left.Right, source.Right));
    }

    private static void Generate(SourceProductionContext ctx,
        AnalyzerConfigOptionsProvider options,
        Compilation compilation,
        ImmutableArray<IRegistration> registrations)
    {
        var rootNamespace = compilation.AssemblyName?.Replace(' ', '_');

        if (rootNamespace is null)
        {
            ctx.ReportDiagnostic(Diagnostics.ECHDI02(DiagnosticSeverity.Error));
            return;
        }

        var moduleContext = new ModuleContext(
            $"{rootNamespace}.Enhanced.DependencyInjection",
            ctx.ReportDiagnostic,
            ctx.CancellationToken
        );

        var referenceModules = GetReferenceModules(compilation, ctx.CancellationToken);

        ctx.AddSource(
            $"{TN.ModuleClass}.g.cs",
            GetModuleText(referenceModules, registrations, moduleContext));

        ctx.AddSource(
            $"{TN.ModuleClass}.Extensions.g.cs",
            GetModuleExtensionsText(moduleContext.Ns));
    }

    private static List<INamedTypeSymbol> GetReferenceModules(
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        var referenceModules = new List<INamedTypeSymbol>();

        foreach (var reference in compilation.SourceModule.ReferencedAssemblySymbols)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!TryGetContainerModule(reference, out var typeSymbol))
                continue;

            referenceModules.Add(typeSymbol);
        }

        return referenceModules;
    }

    private static string GetModuleText(
        IReadOnlyCollection<INamedTypeSymbol> modules,
        ImmutableArray<IRegistration> registrations,
        ModuleContext ctx)
    {
        using var textWriter = new StringWriter();
        using var indentedWriter = new IndentedTextWriter(textWriter);

        indentedWriter.WriteLine("using {0};", TN.ExtensionsNamespace);
        indentedWriter.WriteLine();

        using (indentedWriter.BeginScope("namespace {0}", ctx.Ns))
        {
            indentedWriter.WriteLine("[{0}(\"{1}\", \"{2}\")]", TN.GlobGeneratedCodeAttribute, ToolName, ToolVersion);

            using (indentedWriter.BeginScope("public sealed class {0} : {1}", TN.ModuleClass, TN.GlobModuleInterface))
            using (indentedWriter.BeginScope("public void AddEntries({0} sc)", TN.GlobServiceCollectionInterface))
            {
                foreach (var module in modules)
                {
                    ctx.CancellationToken.ThrowIfCancellationRequested();
                    WriteModule(module, indentedWriter);
                }

                foreach (var registration in registrations)
                {
                    ctx.CancellationToken.ThrowIfCancellationRequested();
                    registration.Write(indentedWriter, ctx);
                }
            }
        }

        indentedWriter.Flush();
        textWriter.Flush();
        return textWriter.ToString();
    }

    private static void WriteModule(ISymbol module, TextWriter writer)
    {
        writer.WriteLine(
            "sc.Module<{0}>();",
            module.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
    }

    private static IRegistration? GetRegistration(GeneratorSyntaxContext ctx, CancellationToken cancellationToken)
    {
        var model = ctx.SemanticModel;
        var classDeclaration = (ClassDeclarationSyntax) ctx.Node;

        IRegistration? result = null;

        foreach (var attributeList in classDeclaration.AttributeLists)
        foreach (var attribute in attributeList.Attributes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var attributeType = attribute.GetSymbolType(model);
            var registration = attributeType switch
            {
                {Name: TN.ContainerEntryAttributeName, IsGenericType: false}
                    => EntryRegistration.CreateFromPlainEntryAttribute(
                        attribute, classDeclaration, ctx),
                {Name: TN.ContainerEntryAttributeName, IsGenericType: true}
                    => EntryRegistration.CreateFromGenericEntryAttribute(
                        attribute, attributeType, classDeclaration, ctx),

                {Name: TN.SingletonAttributeName, IsGenericType: false}
                    => EntryRegistration.CreateFromPlainAttribute(
                        ServiceLifetime.Singleton, attribute, classDeclaration, ctx),
                {Name: TN.SingletonAttributeName, IsGenericType: true}
                    => EntryRegistration.CreateFromGenericAttribute(
                        ServiceLifetime.Singleton, attributeType, classDeclaration),

                {Name: TN.ScopedAttributeName, IsGenericType: false}
                    => EntryRegistration.CreateFromPlainAttribute(
                        ServiceLifetime.Scoped, attribute, classDeclaration, ctx),
                {Name: TN.ScopedAttributeName, IsGenericType: true}
                    => EntryRegistration.CreateFromGenericAttribute(
                        ServiceLifetime.Scoped, attributeType, classDeclaration),

                {Name: TN.TransientAttributeName, IsGenericType: false}
                    => EntryRegistration.CreateFromPlainAttribute(
                        ServiceLifetime.Transient, attribute, classDeclaration, ctx),
                {Name: TN.TransientAttributeName, IsGenericType: true}
                    => EntryRegistration.CreateFromGenericAttribute(
                        ServiceLifetime.Transient, attributeType, classDeclaration),

                _ => null
            };

            if (registration is null)
                continue;

            if (result is not null)
                return new ErrorRegistration(Diagnostics.ECHDI04(attribute.GetLocation(), DiagnosticSeverity.Error));

            result = registration;
        }

        return result;
    }

    private static bool TryGetContainerModule(
        IAssemblySymbol assemblySymbol,
        [NotNullWhen(true)] out INamedTypeSymbol? moduleSymbol)
    {
        var assemblyModuleName = $"{assemblySymbol.Name}.Enhanced.DependencyInjection.{TN.ModuleClass}";
        moduleSymbol = assemblySymbol.GetTypeByMetadataName(assemblyModuleName);
        return moduleSymbol != null;
    }

    private static string GetModuleExtensionsText(string ns)
    {
        return @$"using Enhanced.DependencyInjection.Extensions;

namespace {ns} {{
    /// <summary>
    /// Add generated module of current assembly to <see cref=""IServiceCollection"" />.
    /// </summary>
    /// <param name=""this"">Collection of service descriptors.</param>
    /// <returns>Collection of service descriptors.</returns>
    internal static class {TN.ModuleClass}Extensions {{
        public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddEnhancedModules(this global::Microsoft.Extensions.DependencyInjection.IServiceCollection @this) {{
            @this.Module<ContainerModule>();
            return @this;
        }}
    }}
}}";
    }
}