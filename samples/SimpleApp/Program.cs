using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Host
    .CreateDefaultBuilder()
    .ConfigureServices(c => c.AddEnhancedModules())
    .RunConsoleAsync();