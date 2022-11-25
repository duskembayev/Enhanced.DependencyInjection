namespace Enhanced.DependencyInjection.CodeGeneration;

internal sealed class ModuleContext
{
    private readonly SourceProductionContext _productionContext;

    public ModuleContext(string ns, SourceProductionContext productionContext)
    {
        _productionContext = productionContext;
        Ns = ns;
    }

    public string Ns { get; }
    public CancellationToken CancellationToken => _productionContext.CancellationToken;

    public void Report(Diagnostic diagnostic) => _productionContext.ReportDiagnostic(diagnostic);

    public void AddSource(string hintName, string source) => _productionContext.AddSource(hintName, source);
}