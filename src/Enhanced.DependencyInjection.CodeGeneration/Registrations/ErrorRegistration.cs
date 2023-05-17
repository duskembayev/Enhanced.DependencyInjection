namespace Enhanced.DependencyInjection.CodeGeneration.Registrations;

internal sealed class ErrorRegistration : IRegistration
{
    private readonly Diagnostic _diagnostic;

    internal ErrorRegistration(Diagnostic diagnostic)
    {
        _diagnostic = diagnostic;
    }

    void IRegistration.Write(TextWriter writer, ModuleContext ctx)
    {
        ctx.Report(_diagnostic);
    }
}