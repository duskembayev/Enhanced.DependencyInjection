using System.CodeDom.Compiler;
using System.Text;

namespace Enhanced.DependencyInjection.CodeGeneration.Builders;

internal class FactoryBuilder
{
    public FactoryBuilder(string ns, string subjectInterface, string subjectType)
    {
        Ns = ns;
        SubjectInterface = subjectInterface;
        SubjectType = subjectType;
        FactoryInterface = subjectInterface + "Factory";
        FactoryType = subjectType + "Factory";
        Parameters = new List<(string Type, string Name, bool IsArgument)>();
    }

    public string Ns { get; set; }
    public string SubjectInterface { get; set; }
    public string SubjectType { get; set; }
    public string FactoryInterface { get; set; }
    public string FactoryType { get; set; }
    public IList<(string Type, string Name, bool IsArgument)> Parameters { get; }

    public string Build()
    {
        var methodArgumentBuilder = new StringBuilder();
        var ctorArgumentBuilder = new StringBuilder();
        var ctorBodyBuilder = new StringBuilder();
        var fieldBuilder = new StringBuilder();
        var parameterBuilder = new StringBuilder();

        foreach (var (type, name, isArgument) in Parameters)
        {
            if (parameterBuilder.Length > 0)
                parameterBuilder.Append(", ");

            parameterBuilder.Append(name);

            if (isArgument)
            {
                AddArgument(methodArgumentBuilder, type, name);
                continue;
            }

            fieldBuilder.AppendLine($"private readonly {type} {name};");
            ctorBodyBuilder.AppendLine($"this.{name} = {name};");

            AddArgument(ctorArgumentBuilder, type, name);
        }

        using var textWriter = new StringWriter();
        using var indentedWriter = new IndentedTextWriter(textWriter);

        using (indentedWriter.BeginScope("namespace {0}", Ns))
        {
            indentedWriter.WriteLine("[{0}(\"{1}\", \"{2}\")]", TN.GlobGeneratedCodeAttribute, Tool.Name, Tool.Version);

            using (indentedWriter.BeginScope("public interface {0}", FactoryInterface))
            {
                indentedWriter.Write("{0} Create(", SubjectInterface);
                indentedWriter.Write(methodArgumentBuilder.ToString());
                indentedWriter.WriteLine(");");
            }

            indentedWriter.WriteVerticalTab();
            indentedWriter.WriteLine("[{0}(\"{1}\", \"{2}\")]", TN.GlobGeneratedCodeAttribute, Tool.Name, Tool.Version);

            using (indentedWriter.BeginScope("public sealed class {0} : {1}", FactoryType, FactoryInterface))
            {
                indentedWriter.WriteMultiline(fieldBuilder.ToString());

                indentedWriter.WriteVerticalTab();
                indentedWriter.Write("public {0}(", SubjectType);
                indentedWriter.Write(ctorArgumentBuilder.ToString());
                indentedWriter.WriteLine(")");

                using (indentedWriter.BeginScope())
                {
                    indentedWriter.WriteMultiline(ctorBodyBuilder.ToString());
                }

                indentedWriter.WriteVerticalTab();
                indentedWriter.Write("public {0} Create(", SubjectInterface);
                indentedWriter.Write(methodArgumentBuilder.ToString());
                indentedWriter.WriteLine(")");

                using (indentedWriter.BeginScope())
                {
                    indentedWriter.Write("return new {0}(", SubjectType);
                    indentedWriter.Write(parameterBuilder.ToString());
                    indentedWriter.WriteLine(");");
                }
            }
        }
    }

    private static void AddArgument(StringBuilder builder, string type, string name)
    {
        if (builder.Length > 0)
            builder.Append(", ");

        builder.Append(type);
        builder.Append(" ");
        builder.Append(name);
    }
}