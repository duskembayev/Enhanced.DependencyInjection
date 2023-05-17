using Microsoft.CodeAnalysis.Diagnostics;

namespace Enhanced.DependencyInjection.CodeGeneration.Extensions;

internal static class AnalyzerConfigOptionsProviderExtensions
{
    internal static string GetModuleNamespace(this AnalyzerConfigOptionsProvider @this, SourceProductionContext ctx)
    {
        if (@this.GlobalOptions.TryGetValue("build_property.ModuleNamespace", out var propertyValue)
            && !string.IsNullOrEmpty(propertyValue))
            return propertyValue;

        ctx.ReportDiagnostic(Diagnostics.ECHDI05("ModuleNamespace", DiagnosticSeverity.Warning));
        return "Enhanced_" + Guid.NewGuid().ToString("D").Substring(0, 8);
    }

    internal static string GetModuleName(this AnalyzerConfigOptionsProvider @this, SourceProductionContext ctx)
    {
        if (@this.GlobalOptions.TryGetValue("build_property.ModuleClassName", out var propertyValue)
            && !string.IsNullOrEmpty(propertyValue))
            return propertyValue;

        ctx.ReportDiagnostic(Diagnostics.ECHDI05("ModuleClassName", DiagnosticSeverity.Warning));
        return "ContainerModule";
    }

    internal static string? GetModuleRegistrationMethodName(
        this AnalyzerConfigOptionsProvider @this,
        SourceProductionContext ctx)
    {
        bool? generate = null;

        if (@this.GlobalOptions.TryGetValue("build_property.GenerateModuleRegistrationMethod", out var propertyValue)
            && !string.IsNullOrEmpty(propertyValue)
            && bool.TryParse(propertyValue, out var flag))
        {
            generate = flag;
        }

        if (generate is null)
        {
            ctx.ReportDiagnostic(Diagnostics.ECHDI05("GenerateModuleRegistrationMethod", DiagnosticSeverity.Warning));
            generate = true;
        }

        if (!generate.Value)
            return null;

        if (@this.GlobalOptions.TryGetValue("build_property.ModuleRegistrationMethodName", out propertyValue)
            && !string.IsNullOrEmpty(propertyValue))
            return propertyValue;

        ctx.ReportDiagnostic(Diagnostics.ECHDI05("ModuleRegistrationMethodName", DiagnosticSeverity.Warning));
        return "AddEnhancedModules";
    }
}