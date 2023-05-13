namespace Enhanced.DependencyInjection;

/// <summary>
///     Attribute to declare implementation entry with scoped lifetime.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ScopedAttribute : Attribute
{
    /// <summary>
    ///     Define implemented services.
    /// </summary>
    public ScopedAttribute()
    {
        Interface0 = null;
        Interfaces = Array.Empty<Type>();
    }

    /// <summary>
    ///     Define implemented service.
    /// </summary>
    /// <param name="interface0">
    ///     The Type of the service.
    /// </param>
    public ScopedAttribute(Type interface0)
    {
        Interface0 = interface0;
        Interfaces = Array.Empty<Type>();
    }

    /// <summary>
    ///     Define implemented services.
    /// </summary>
    /// <param name="interface0">
    ///     The Type of the service.
    /// </param>
    /// <param name="interfaces">
    ///     Additional types of the service.
    /// </param>
    public ScopedAttribute(Type interface0, params Type[] interfaces)
    {
        Interface0 = interface0;
        Interfaces = interfaces;
    }

    internal Type? Interface0 { get; }
    internal Type[] Interfaces { get; }
}