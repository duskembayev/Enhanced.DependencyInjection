using System.CodeDom.Compiler;

namespace Enhanced.DependencyInjection.CodeGeneration.Writers;

internal sealed class CSharpScope : IDisposable
{
    private readonly IndentedTextWriter _indentedTextWriter;

    public CSharpScope(IndentedTextWriter indentedTextWriter)
    {
        _indentedTextWriter = indentedTextWriter;
        _indentedTextWriter.WriteLine("{");
        _indentedTextWriter.Indent++;
    }
    
    public void Dispose()
    {
        _indentedTextWriter.Indent--;
        _indentedTextWriter.WriteLine("}");
    }
}