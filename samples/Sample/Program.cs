using Enhanced.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Sample;

public static class Program
{
    public static async Task Main()
    {
        await Host
            .CreateDefaultBuilder()
            .ConfigureServices(c => c.AddEnhancedModules())
            .RunConsoleAsync();
    }
}