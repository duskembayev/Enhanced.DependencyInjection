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
    private const string ModuleInterface = "Enhanced.DependencyInjection.IContainerModule";
    private const string ModuleClass = "ContainerModule";

    private const string ContainerEntryAttribute = "Enhanced.DependencyInjection.ContainerEntryAttribute";
    private const string GeneratedCodeAttribute = "System.CodeDom.Compiler.GeneratedCodeAttribute";

    private const string ServiceCollectionInterface = "Microsoft.Extensions.DependencyInjection.IServiceCollection";
    private const string ServiceLifetimeEnum = "Microsoft.Extensions.DependencyInjection.ServiceLifetime";

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
        var generatedCodeAttributeName = compilation
            .GetTypeByMetadataName(GeneratedCodeAttribute)
            ?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        if (generatedCodeAttributeName is null)
            ctx.ReportDiagnostic(Diagnostics.ECHDI01(GeneratedCodeAttribute, DiagnosticSeverity.Warning));

        var moduleInterfaceName = compilation
            .GetTypeByMetadataName(ModuleInterface)
            ?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        if (moduleInterfaceName is null)
        {
            ctx.ReportDiagnostic(Diagnostics.ECHDI01(ModuleInterface, DiagnosticSeverity.Error));
            return;
        }

        var serviceCollectionName = compilation
            .GetTypeByMetadataName(ServiceCollectionInterface)?
            .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        if (serviceCollectionName is null)
        {
            ctx.ReportDiagnostic(Diagnostics.ECHDI01(ServiceCollectionInterface, DiagnosticSeverity.Error));
            return;
        }

        var serviceLifetimeName = compilation
            .GetTypeByMetadataName(ServiceLifetimeEnum)?
            .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        if (serviceLifetimeName is null)
        {
            ctx.ReportDiagnostic(Diagnostics.ECHDI01(ServiceLifetimeEnum, DiagnosticSeverity.Error));
            return;
        }

        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream);
        using var indentedWriter = new IndentedTextWriter(streamWriter);

        indentedWriter.WriteLine("using Enhanced.DependencyInjection.Extensions;");
        indentedWriter.WriteLine();

        using (indentedWriter.BeginScope("namespace {0}", ModuleNamespace))
        {
            if (generatedCodeAttributeName != null)
                indentedWriter.WriteLine("[{0}(\"{1}\", \"{2}\")]", generatedCodeAttributeName, ToolName, ToolVersion);

            using (indentedWriter.BeginScope("internal sealed class {0} : {1}", ModuleClass, moduleInterfaceName))
            using (indentedWriter.BeginScope("public void AddEntries({0} sc)", serviceCollectionName))
            {
                foreach (var registration in registrations)
                {
                    ctx.CancellationToken.ThrowIfCancellationRequested();
                    WriteRegistration(registration, indentedWriter, serviceLifetimeName);
                }
            }
        }

        indentedWriter.Flush();
        streamWriter.Flush();
        memoryStream.Flush();

        memoryStream.Seek(0, SeekOrigin.Begin);

        ctx.AddSource(
            $"{ModuleClass}.g.cs",
            SourceText.From(memoryStream, Encoding.UTF8, canBeEmbedded: true, throwIfBinaryDetected: true));
    }

    private static void WriteRegistration(
        DiRegistration registration,
        TextWriter indentedWriter,
        string serviceLifetimeName)
    {
        var ns = registration.ImplType.GetNamespace();

        indentedWriter.Write("sc.Entry<global::{0}.{1}>(", ns, registration.ImplType.Identifier.ValueText);
        indentedWriter.Write("{0}.{1:G}", serviceLifetimeName, registration.Lifetime);

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
                .Select(t => ModelExtensions.GetTypeInfo(model, t).Type!)
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