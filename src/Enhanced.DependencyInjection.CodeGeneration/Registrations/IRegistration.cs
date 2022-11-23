namespace Enhanced.DependencyInjection.CodeGeneration.Registrations;

internal interface IRegistration
{
    void Write(TextWriter writer, ModuleContext ctx);
}