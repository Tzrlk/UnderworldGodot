using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Underworld.utility;

// Creates a single entrypoint for dependency injection across the whole application.
public static class Injection
{

    public delegate void ServicesConfigurator(IServiceCollection services);

    public class InjectionNotConfiguredException(string msg) : Exception(msg);

    private static ServiceProvider provider = null;
    private static ServiceProvider Provider => provider
        ?? throw new InjectionNotConfiguredException("Injection has not been configured yet!");

    public static async Task<IServiceProvider> Configure(ServicesConfigurator configurator)
    {
        // Start disposing of any previous providers if they exist.
        var disposal = provider?.DisposeAsync();

        // Create a new services collection and pass it out to configure.
        var services = new ServiceCollection();
        configurator(services);

        // Compile the configured services into a provider.
        provider = services.BuildServiceProvider();

        // Wait for any disposals from the previous provider to finish.
        await disposal.GetValueOrDefault();

        // Return the provider we compiled.
        return provider;
    }

    /// <see cref="IServiceProvider.GetRequiredService()" />
    /// <see cref="IServiceProvider.GetService()" />
    public static T GetService<T>(bool required = false)
        => required
            ? Provider.GetRequiredService<T>()
            : Provider.GetService<T>();

    /// <see cref="IServiceProvider.GetRequiredKeyedService(object? serviceKey)" />
    /// <see cref="IServiceProvider.GetKeyedService(object? serviceKey)" />
    public static T GetService<T>(object key, bool required = false)
        => required
            ? Provider.GetRequiredKeyedService<T>(key)
            : Provider.GetKeyedService<T>(key);

    /// <see cref="IServiceProvider.GetServices()" />
    public static IEnumerable<T> GetServices<T>()
        => Provider.GetServices<T>();

    /// <see cref="IServiceProvider.GetKeyedServices(object? serviceKey)" />
    public static IEnumerable<T> GetServices<T>(object key)
        => Provider.GetKeyedServices<T>(key);

    /// <see cref="IServiceProvider.CreateScope()" />
    public static IServiceScope CreateScope()
        => Provider.CreateScope();

    /// <see cref="IServiceProvider.CreateAsyncScope()" />
    public static AsyncServiceScope CreateAsyncScope()
        => Provider.CreateAsyncScope();

}
