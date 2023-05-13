namespace Enhanced.DependencyInjection.CodeGeneration.Extensions;

internal static class IncrementalValuesProviderExtensions
{
    internal static IncrementalValuesProvider<TSource> WhereNotNull<TSource>(
        this in IncrementalValuesProvider<TSource?> @this)
    {
        return @this
            .Where(static source => source != null)
            .Select(static (source, _) => source!);
    }
}