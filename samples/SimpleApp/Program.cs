using Microsoft.Extensions.Hosting;
using SimpleApp.Enhanced.DependencyInjection;

await Host
    .CreateDefaultBuilder()
    .ConfigureServices(c => c.AddEnhancedModules())
    .RunConsoleAsync();