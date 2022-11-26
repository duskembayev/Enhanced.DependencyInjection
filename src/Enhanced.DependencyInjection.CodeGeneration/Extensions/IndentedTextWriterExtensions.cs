using System.CodeDom.Compiler;
using System.Text;
using Enhanced.DependencyInjection.CodeGeneration.Writers;

namespace Enhanced.DependencyInjection.CodeGeneration.Extensions;

internal static class IndentedTextWriterExtensions
{
    public static IDisposable BeginScope(this IndentedTextWriter @this)
    {
        return new CSharpScope(@this);
    }

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

    public static void WriteMultiline(this IndentedTextWriter @this, string s)
    {
        var stringReader = new StringReader(s);
        var line = stringReader.ReadLine();

        while (line != null)
        {
            @this.WriteLine(line);
            line = stringReader.ReadLine();
        }
    }

    public static void WriteVerticalTab(this @IndentedTextWriter @this)
    {
        @this.WriteLineNoTabs(string.Empty);
    }
}