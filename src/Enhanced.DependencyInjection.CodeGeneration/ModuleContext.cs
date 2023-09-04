namespace Enhanced.DependencyInjection.CodeGeneration;

internal sealed class ModuleContext
{
    private readonly Action<Diagnostic> _diagnosticHandler;

    public ModuleContext(
        string moduleNamespace,
        Compilation compilation,
        Action<Diagnostic> diagnosticHandler, CancellationToken cancellationToken)
    {
        _diagnosticHandler = diagnosticHandler;

        Compilation = compilation;
        CancellationToken = cancellationToken;
        ModuleNamespace = moduleNamespace;
    }

    public Compilation Compilation { get; }
    public string ModuleNamespace { get; }
    public CancellationToken CancellationToken { get; }

    public void Report(Diagnostic diagnostic)
    {
        _diagnosticHandler.Invoke(diagnostic);
    }
}