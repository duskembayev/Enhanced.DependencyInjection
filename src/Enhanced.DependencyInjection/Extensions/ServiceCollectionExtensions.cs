namespace Enhanced.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void Entry<TImpl>(this IServiceCollection @this,
        ServiceLifetime lifetime, Type interface0)
        where TImpl : class
    {
        @this.Add(new ServiceDescriptor(interface0, typeof(TImpl), lifetime));
    }

    public static void Entry<TImpl>(this IServiceCollection @this,
        ServiceLifetime lifetime, Type interface0, params Type[] interfaces)
        where TImpl : class
    {
        @this.Entry<TImpl>(lifetime, interface0);

        foreach (var @interface in interfaces)
            @this.Add(new ServiceDescriptor(@interface, sp => sp.GetRequiredService(interface0), lifetime));
    }
}