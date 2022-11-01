namespace Enhanced.DependencyInjection.CodeGeneration.Extensions;

internal static class EnumerableExtensions
{
    internal static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> @this)
    {
        foreach (var item in @this)
            if (item is not null)
                yield return item;
    }
}