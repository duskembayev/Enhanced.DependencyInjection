using Enhanced.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleRef;

namespace Sample;

[ContainerEntry(ServiceLifetime.Singleton, typeof(ISampleService), typeof(IHostedService))]
internal sealed class SampleService : ISampleService, IHostedService, IDisposable
{
    private const int LoopDelay = 1500;

    private readonly IRandomService _randomService;
    private readonly ILogger<SampleService> _logger;
    private readonly CancellationTokenSource _cts;

    private Task<Task>? _loopTask;

    public SampleService(IRandomService randomService, ILogger<SampleService> logger)
    {
        _randomService = randomService;
        _logger = logger;
        _cts = new CancellationTokenSource();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sample service started.");
        _loopTask = Task.Factory.StartNew(ServiceLoop, TaskCreationOptions.None);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        _loopTask?.Wait(cancellationToken);
        _logger.LogInformation("Sample service stopped.");
        return Task.CompletedTask;
    }

    private async Task ServiceLoop()
    {
        while (!_cts.IsCancellationRequested)
        {
            var i = _randomService.GetInt();
            _logger.LogInformation($"Random number is [{i}]");
            await Task.Delay(LoopDelay, _cts.Token);
        }
    }

    public void Dispose()
    {
        _cts.Dispose();
    }
}