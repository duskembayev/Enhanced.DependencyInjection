using System.CodeDom.Compiler;
using Enhanced.DependencyInjection.CodeGeneration.Writers;

namespace Enhanced.DependencyInjection.CodeGeneration.Extensions;

internal static class IndentedTextWriterExtensions
{
    public static IDisposable BeginScope(this IndentedTextWriter @this, string s)
    {
        @this.WriteLine(s);
        return new CSharpScope(@this);
    }

    public static IDisposable BeginScope(this IndentedTextWriter @this, string format, object arg0)
    {
        @this.WriteLine(format, arg0);
        return new CSharpScope(@this);
    }

    public static IDisposable BeginScope(this IndentedTextWriter @this, string format, object arg0, object arg1)
    {
        @this.WriteLine(format, arg0, arg1);
        return new CSharpScope(@this);
    }
}