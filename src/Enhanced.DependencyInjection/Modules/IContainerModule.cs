using Microsoft.Extensions.Configuration;

namespace Enhanced.DependencyInjection.Modules;

/// <summary>
///     Provides a mechanism for registering assembly entries in <see cref="IServiceCollection" />.
/// </summary>
public interface IContainerModule
{
    /// <summary>
    ///     Add entries to provided <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">
    ///     The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configuration">
    ///     The configuration being bound.
    /// </param>
    void AddEntries(IServiceCollection serviceCollection, IConfiguration? configuration);
}