﻿using Microsoft.Extensions.Hosting;
using Sample.Enhanced.DependencyInjection;

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