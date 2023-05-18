using System.CodeDom.Compiler;
using Enhanced.DependencyInjection.CodeGeneration.Registrations;

namespace Enhanced.DependencyInjection.CodeGeneration;

partial class Generator
{
    private static string GetModuleRegistrationSource(ModuleContext ctx)
    {
        return @$"// <auto-generated />
using global::Enhanced.DependencyInjection.Extensions;
using IServiceCollection = global::Microsoft.Extensions.DependencyInjection.IServiceCollection;

namespace {ctx.Ns}
{{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{ToolName}"", ""{ToolVersion}"")]
    internal static class EnhancedDependencyInjectionExtensions
    {{
        /// <summary>
        /// Add generated module of current assembly to <see cref=""IServiceCollection"" />.
        /// </summary>
        public static IServiceCollection {ctx.MethodName}(this IServiceCollection serviceCollection)
        {{
            serviceCollection.Module<{ctx.ModuleName}>();
            return serviceCollection;
        }}
    }}
}}";
    }

    private static string GetModuleAttributeSource(ModuleContext ctx)
    {
        return @$"// <auto-generated />
using global::Enhanced.DependencyInjection;

[assembly:ContainerModule(typeof({ctx.Ns}.{ctx.ModuleName}))]";
    }

    private static string GetModuleSource(
        ImmutableArray<INamedTypeSymbol> modules,
        ImmutableArray<IRegistration> registrations,
        ModuleContext ctx)
    {
        using var textWriter = new StringWriter();
        using var indentedWriter = new IndentedTextWriter(textWriter);

        indentedWriter.WriteLine(@"// <auto-generated />
using global::Enhanced.DependencyInjection.Extensions;
using IContainerModule = global::Enhanced.DependencyInjection.Modules.IContainerModule;
using IServiceCollection = global::Microsoft.Extensions.DependencyInjection.IServiceCollection;
using ServiceLifetime = global::Microsoft.Extensions.DependencyInjection.ServiceLifetime;");
        indentedWriter.WriteLine();

        using (indentedWriter.BeginScope("namespace {0}", ctx.Ns))
        {
            indentedWriter.WriteLine("[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]",
                ToolName, ToolVersion);

            using (indentedWriter.BeginScope("public sealed class {0} : IContainerModule", ctx.ModuleName))
            using (indentedWriter.BeginScope("public void AddEntries(IServiceCollection serviceCollection)"))
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
            "serviceCollection.Module<{0}>();",
            module.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
    }
}