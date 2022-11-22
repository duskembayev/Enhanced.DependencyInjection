using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace Enhanced.DependencyInjection.CodeGeneration;

[Generator(LanguageNames.CSharp)]
public class ContainerEntryAttributeProcessor : IIncrementalGenerator
{
    private const string ModuleClass = "ContainerModule";
    private const string ContainerEntryAttribute = "Enhanced.DependencyInjection.ContainerEntryAttribute";
    private const string ExtensionsNamespace = "Enhanced.DependencyInjection.Extensions";

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
            .CreateSyntaxProvider(IsClassWithAttribute, GetRegistration)
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
        ImmutableArray<DiRegistration> registrations)
    {
        var rootNamespace = compilation.AssemblyName?.Replace(' ', '_');

        if (rootNamespace is null)
        {
            ctx.ReportDiagnostic(Diagnostics.ECHDI02(DiagnosticSeverity.Error));
            return;
        }

        var ns = $"{rootNamespace}.Enhanced.DependencyInjection";
        var tn = new TypeNames(compilation, ctx.ReportDiagnostic);
        var referenceModules = new List<INamedTypeSymbol>();

        foreach (var reference in compilation.SourceModule.ReferencedAssemblySymbols)
        {
            if (!TryGetContainerModule(reference, out var typeSymbol))
                continue;

            referenceModules.Add(typeSymbol);
        }

        ctx.AddSource(
            $"{ModuleClass}.g.cs",
            GetModuleText(ns, tn, referenceModules, registrations, ctx.CancellationToken));

        ctx.AddSource($"{ModuleClass}.Extensions.g.cs", @$"
using Enhanced.DependencyInjection.Extensions;

namespace {ns} {{
    /// <summary>
    /// Add generated module of current assembly to <see cref=""IServiceCollection"" />.
    /// </summary>
    /// <param name=""this"">Collection of service descriptors.</param>
    /// <returns>Collection of service descriptors.</returns>
    internal static class {ModuleClass}Extensions {{
        public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddEnhancedModules(this global::Microsoft.Extensions.DependencyInjection.IServiceCollection @this) {{
            @this.Module<ContainerModule>();
            return @this;
        }}
    }}
}}");
    }

    private static string GetModuleText(string ns,
        TypeNames tn,
        IReadOnlyCollection<INamedTypeSymbol> modules,
        ImmutableArray<DiRegistration> registrations,
        CancellationToken cancellationToken)
    {
        using var textWriter = new StringWriter();
        using var indentedWriter = new IndentedTextWriter(textWriter);

        indentedWriter.WriteLine("using {0};", ExtensionsNamespace);
        indentedWriter.WriteLine();

        using (indentedWriter.BeginScope("namespace {0}", ns))
        {
            indentedWriter.WriteLine("[{0}(\"{1}\", \"{2}\")]", tn.GeneratedCodeAttributeName, ToolName, ToolVersion);

            using (indentedWriter.BeginScope("public sealed class {0} : {1}", ModuleClass, tn.ModuleInterfaceName))
            using (indentedWriter.BeginScope("public void AddEntries({0} sc)", tn.ServiceCollectionName))
            {
                foreach (var module in modules)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    WriteModule(module, indentedWriter);
                }

                foreach (var registration in registrations)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    WriteRegistration(registration, indentedWriter, tn.ServiceLifetimeEnumName);
                }
            }
        }

        indentedWriter.Flush();
        textWriter.Flush();
        return textWriter.ToString();
    }

    private static void WriteModule(INamedTypeSymbol module, IndentedTextWriter indentedWriter)
    {
        indentedWriter.WriteLine(
            "sc.Module<{0}>();",
            module.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
    }

    private static void WriteRegistration(
        DiRegistration registration,
        TextWriter indentedWriter,
        string serviceLifetimeEnumName)
    {
        var ns = registration.ImplType.GetNamespace();

        indentedWriter.Write("sc.Entry<global::{0}.{1}>(", ns, registration.ImplType.Identifier.ValueText);
        indentedWriter.Write("{0}.{1:G}", serviceLifetimeEnumName, registration.Lifetime);

        foreach (var @interface in registration.Interfaces)
        {
            var interfaceFullName = @interface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            indentedWriter.Write(", typeof({0})", interfaceFullName);
        }

        indentedWriter.WriteLine(");");
    }

    private static DiRegistration? GetRegistration(GeneratorSyntaxContext ctx, CancellationToken cancellationToken)
    {
        var model = ctx.SemanticModel;
        var classDeclaration = (ClassDeclarationSyntax) ctx.Node;

        foreach (var attributeList in classDeclaration.AttributeLists)
        foreach (var attribute in attributeList.Attributes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (attribute.ArgumentList is not {Arguments.Count: >= 2})
                continue;

            if (!attribute.IsTypeFullName(model, ContainerEntryAttribute))
                continue;

            if (!attribute.TryGetEnumValue<ServiceLifetime>(0, model, out var serviceLifetime))
                continue;

            var interfaces = attribute.ArgumentList.Arguments
                .FindTypeOfExpressions(1)
                .Select(t => model.GetTypeInfo(t).Type!)
                .ToImmutableArray();

            if (interfaces.Length == 0)
                continue;

            return new DiRegistration(serviceLifetime.Value, classDeclaration, interfaces);
        }

        return null;
    }

    private static bool IsClassWithAttribute(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax {AttributeLists.Count: > 0};
    }

    private static bool TryGetContainerModule(
        IAssemblySymbol assemblySymbol,
        [NotNullWhen(true)] out INamedTypeSymbol? moduleSymbol)
    {
        var assemblyModuleName = $"{assemblySymbol.Name}.Enhanced.DependencyInjection.{ModuleClass}";
        moduleSymbol = assemblySymbol.GetTypeByMetadataName(assemblyModuleName);
        return moduleSymbol != null;
    }
}