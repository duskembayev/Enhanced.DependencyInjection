namespace Enhanced.DependencyInjection;

/// <summary>
///     Attribute to declare implementation entry with singleton lifetime and implemented service.
/// </summary>
/// <typeparam name="TInterface">
///     The Type of the service.
/// </typeparam>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SingletonAttribute<TInterface> : Attribute
{
}

/// <summary>
///     Attribute to declare implementation entry with singleton lifetime and implemented services.
/// </summary>
/// <typeparam name="TInterface1">
///     First type of the service.
/// </typeparam>
/// <typeparam name="TInterface2">
///     Second type of the service.
/// </typeparam>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SingletonAttribute<TInterface1, TInterface2> : Attribute
{
}

/// <summary>
///     Attribute to declare implementation entry with singleton lifetime and implemented services.
/// </summary>
/// <typeparam name="TInterface1">
///     First type of the service.
/// </typeparam>
/// <typeparam name="TInterface2">
///     Second type of the service.
/// </typeparam>
/// <typeparam name="TInterface3">
///     Third type of the service.
/// </typeparam>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SingletonAttribute<TInterface1, TInterface2, TInterface3> : Attribute
{
}