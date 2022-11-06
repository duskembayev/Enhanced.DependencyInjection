namespace Enhanced.DependencyInjection.Modules;

/// <summary>
/// Provides a mechanism for registering assembly entries in <see cref="IServiceCollection" />.
/// </summary>
public interface IContainerModule
{
    /// <summary>
    /// Add entries to provided <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">>Collection of service descriptors.</param>
    void AddEntries(IServiceCollection serviceCollection);
}