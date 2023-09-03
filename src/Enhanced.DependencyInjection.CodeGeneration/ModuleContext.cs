namespace Enhanced.DependencyInjection.CodeGeneration;

internal sealed class ModuleContext
{
    private readonly Action<Diagnostic> _diagnosticHandler;

    public ModuleContext(
        Compilation compilation,
        Action<Diagnostic> diagnosticHandler,
        CancellationToken cancellationToken)
    {
        _diagnosticHandler = diagnosticHandler;

        Compilation = compilation;
        CancellationToken = cancellationToken;
    }

    public Compilation Compilation { get; }
    public CancellationToken CancellationToken { get; }

    public void Report(Diagnostic diagnostic)
    {
        _diagnosticHandler.Invoke(diagnostic);
    }
}