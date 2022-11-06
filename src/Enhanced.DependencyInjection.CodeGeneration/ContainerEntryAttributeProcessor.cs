using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace Enhanced.DependencyInjection.CodeGeneration;

[Generator(LanguageNames.CSharp)]
public class ContainerEntryAttributeProcessor : IIncrementalGenerator
{
    private const string ModuleNamespace = "Enhanced.DependencyInjection.g";
    private const string ModuleClass = "ContainerModule";
    private const string ModuleRefAttribute = "Enhanced.DependencyInjection.Modules.ContainerModuleAttribute";

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
        var tn = new TypeNames(compilation, ctx.ReportDiagnostic);

        ctx.AddSource($"{ModuleClass}.g.cs", GetModuleText(tn, registrations, ctx.CancellationToken));
        ctx.AddSource(
            $"{ModuleClass}.asm.g.cs",
            $"[assembly: {ModuleRefAttribute}(typeof({ModuleNamespace}.{ModuleClass}))]");
    }

    private static string GetModuleText(
        TypeNames tn,
        ImmutableArray<DiRegistration> registrations,
        CancellationToken cancellationToken)
    {
        using var textWriter = new StringWriter();
        using var indentedWriter = new IndentedTextWriter(textWriter);

        indentedWriter.WriteLine("using {0};", ExtensionsNamespace);
        indentedWriter.WriteLine();

        using (indentedWriter.BeginScope("namespace {0}", ModuleNamespace))
        {
            indentedWriter.WriteLine("[{0}(\"{1}\", \"{2}\")]", tn.GeneratedCodeAttributeName, ToolName, ToolVersion);

            using (indentedWriter.BeginScope("internal sealed class {0} : {1}", ModuleClass, tn.ModuleInterfaceName))
            using (indentedWriter.BeginScope("public void AddEntries({0} sc)", tn.ServiceCollectionName))
            {
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
}