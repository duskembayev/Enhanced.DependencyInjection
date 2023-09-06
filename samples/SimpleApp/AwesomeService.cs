using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace SimpleApp;

[Singleton<IHostedService>]
internal sealed class AwesomeService : IHostedService
{
    private readonly IOptions<AwesomeOptions> _options;

    public AwesomeService(IOptions<AwesomeOptions> options)
    {
        _options = options;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine(_options.Value.EntryMessage);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine(_options.Value.ExitMessage);
        return Task.CompletedTask;
    }
}