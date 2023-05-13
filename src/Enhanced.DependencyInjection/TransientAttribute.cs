namespace Enhanced.DependencyInjection;

/// <summary>
///     Attribute to declare implementation entry with transient lifetime.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class TransientAttribute : Attribute
{
    /// <summary>
    ///   Define implemented services.
    /// </summary>
    public TransientAttribute()
    {
        Interface0 = null;
        Interfaces = Array.Empty<Type>();
    }

    /// <summary>
    ///   Define implemented service.
    /// </summary>
    /// <param name="interface0">
    ///    The Type of the service.
    /// </param>
    public TransientAttribute(Type interface0)
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
    public TransientAttribute(Type interface0, params Type[] interfaces)
    {
        Interface0 = interface0;
        Interfaces = interfaces;
    }

    internal Type? Interface0 { get; }
    internal Type[] Interfaces { get; }
}
