namespace Enhanced.DependencyInjection.CodeGeneration;

internal sealed class ModuleContext
{
    private readonly Action<Diagnostic> _diagnosticHandler;

    public ModuleContext(string ns,
        string moduleName,
        string? methodName,
        Action<Diagnostic> diagnosticHandler,
        CancellationToken cancellationToken)
    {
        _diagnosticHandler = diagnosticHandler;

        Ns = ns;
        ModuleName = moduleName;
        MethodName = methodName;

        CancellationToken = cancellationToken;
    }

    public string Ns { get; }
    public string ModuleName { get; }
    public string? MethodName { get; }

    public CancellationToken CancellationToken { get; }

    public void Report(Diagnostic diagnostic)
    {
        _diagnosticHandler.Invoke(diagnostic);
    }
}