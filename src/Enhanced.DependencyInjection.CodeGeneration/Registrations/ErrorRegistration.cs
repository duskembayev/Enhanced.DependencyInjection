namespace Enhanced.DependencyInjection.CodeGeneration.Registrations;

internal sealed class ErrorRegistration : RegistrationWithDiagnostics
{
    public ErrorRegistration(Diagnostic diagnostic) => Report(diagnostic);
}