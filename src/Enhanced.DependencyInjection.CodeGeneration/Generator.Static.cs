using System.Reflection;

namespace Enhanced.DependencyInjection.CodeGeneration;

partial class Generator
{
    private static readonly string ToolName;
    private static readonly string ToolVersion;

    static Generator()
    {
        var assemblyName = Assembly
            .GetExecutingAssembly()
            .GetName();

        ToolName = assemblyName.Name;
        ToolVersion = assemblyName.Version.ToString(3);
    }
}