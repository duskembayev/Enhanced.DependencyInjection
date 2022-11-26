using System.Reflection;

namespace Enhanced.DependencyInjection.CodeGeneration;

public static class Tool
{
    public static readonly string Name;
    public static readonly string Version;

    static Tool()
    {
        var assemblyName = Assembly
            .GetExecutingAssembly()
            .GetName();

        Name = assemblyName.Name;
        Version = assemblyName.Version.ToString(3);
    }
}