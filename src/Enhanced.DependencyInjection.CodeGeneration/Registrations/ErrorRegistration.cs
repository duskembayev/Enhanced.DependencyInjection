namespace Enhanced.DependencyInjection.CodeGeneration.Registrations;

internal sealed class ErrorRegistration : IRegistration
{
    private readonly Diagnostic _diagnostic;

    public ErrorRegistration(Diagnostic diagnostic)
    {
        _diagnostic = diagnostic;
    }

    public void Write(TextWriter writer, ModuleContext ctx)
    {
        ctx.Report(_diagnostic);
    }
}