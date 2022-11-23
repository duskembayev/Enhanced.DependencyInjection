using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using Enhanced.DependencyInjection.CodeGeneration.Registrations;

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
#if DEBUG
        Debugger.Launch();
#endif
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider((n, t) => n.IsClassWithAttribute(t), GetRegistration)
            .WhereNotNull()
            .Collect();

        var classesPerCompilation = context.CompilationProvider.Combine(classDeclarations);

        context.RegisterSourceOutput(
            classesPerCompilation,
            static (ctx, source) => Generate(ctx, source.Left, source.Right));
    }

    private static void Generate(
        SourceProductionContext ctx,
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

        foreach (var attributeList in classDeclaration.AttributeLists)
        foreach (var attribute in attributeList.Attributes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var attributeType = attribute.GetTypeFullName(model);
            var registration = attributeType switch
            {
                TN.ContainerEntryAttribute => EntryRegistration.Create(attribute, classDeclaration, ctx),
                _ => null
            };

            if (registration is null)
                continue;

            return registration;
        }

        return null;
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