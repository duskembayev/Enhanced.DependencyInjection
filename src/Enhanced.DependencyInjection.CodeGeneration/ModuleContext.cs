namespace Enhanced.DependencyInjection.CodeGeneration;

internal sealed class ModuleContext
{
    private readonly Action<Diagnostic> _diagnosticHandler;

    public ModuleContext(string ns,
        string moduleName,
        string? methodName,
        Compilation compilation,
        Action<Diagnostic> diagnosticHandler,
        CancellationToken cancellationToken)
    {
        _diagnosticHandler = diagnosticHandler;

        Ns = ns;
        ModuleName = moduleName;
        MethodName = methodName;
        Compilation = compilation;

        CancellationToken = cancellationToken;
    }

    public string Ns { get; }
    public string ModuleName { get; }
    public string? MethodName { get; }
    public Compilation Compilation { get; }
    public CancellationToken CancellationToken { get; }

    public void Report(Diagnostic diagnostic)
    {
        _diagnosticHandler.Invoke(diagnostic);
    }
}