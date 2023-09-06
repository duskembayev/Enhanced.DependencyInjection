namespace Enhanced.DependencyInjection;

/// <summary>
///     Attribute to register a configuration instance which target type will bind against.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class OptionsAttribute : Attribute
{
    /// <summary>
    ///     Register a configuration instance which target type will bind against.
    /// </summary>
    public OptionsAttribute()
    {
    }

    /// <summary>
    ///     Register a configuration instance which target type will bind against.
    /// </summary>
    /// <param name="key">
    ///     The key of the configuration section.
    /// </param>
    public OptionsAttribute(string key)
    {
        Key = key;
    }

    internal string? Key { get; }
}