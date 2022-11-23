namespace Enhanced.DependencyInjection.CodeGeneration;

internal sealed class ModuleContext
{
    private readonly Action<Diagnostic> _diagnosticHandler;

    public ModuleContext(
        string ns,
        Action<Diagnostic> diagnosticHandler,
        CancellationToken cancellationToken)
    {
        _diagnosticHandler = diagnosticHandler;
        Ns = ns;
        CancellationToken = cancellationToken;
    }

    public string Ns { get; }
    public CancellationToken CancellationToken { get; }

    public void Report(Diagnostic diagnostic)
    {
        _diagnosticHandler.Invoke(diagnostic);
    }
}