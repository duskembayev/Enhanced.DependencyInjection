using Enhanced.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SimpleApp;

[Singleton<IHostedService>]
class AwesomeService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Hello world!");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("I'll be back!");
        return Task.CompletedTask;
    }
}