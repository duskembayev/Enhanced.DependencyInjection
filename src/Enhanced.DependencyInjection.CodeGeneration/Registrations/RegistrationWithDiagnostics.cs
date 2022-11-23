namespace Enhanced.DependencyInjection.CodeGeneration.Registrations;

internal abstract class RegistrationWithDiagnostics : IRegistration
{
    private readonly List<Diagnostic> _diagnostics = new();

    protected bool HasErrors { get; private set; }

    public virtual void Write(TextWriter writer, ModuleContext ctx)
    {
        foreach (var diagnostic in _diagnostics)
            ctx.Report(diagnostic);
    }

    protected void Report(Diagnostic diagnostic)
    {
        _diagnostics.Add(diagnostic);
        HasErrors |= diagnostic.Severity == DiagnosticSeverity.Error;
    }
}