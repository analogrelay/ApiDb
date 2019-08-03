using System;
using System.CommandLine;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Contrib.Extensions.Hosting.Tool
{
    public static class ToolHost
    {
        public static IHostBuilder CreateDefaultBuilder(string[] args, string configEnvironmentVariablePrefix = null)
        {
            // We assume that a tool doesn't want App Configuration. That may be wrong. The user is always welcome to call '.ConfigureAppConfiguration'

            var diag = false;

            // We could consider putting this in release builds as well, but System.CommandLine already supports it in the default case.
#if DEBUG
            // Handle the debug directive (System.CommandLine handles it too, but we want to support it even earlier)
            if(args.Any(a => string.Equals(a, "[debug]", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"Ready for debugger to attach. Process ID: {System.Diagnostics.Process.GetCurrentProcess().Id}");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();

                args = args.Where(a => !string.Equals(a, "[debug]", StringComparison.OrdinalIgnoreCase)).ToArray();
            }

            // Handle the diag directive
            if(args.Any(a => string.Equals(a, "[diag]", StringComparison.OrdinalIgnoreCase)))
            {
                diag = true;
                args = args.Where(a => !string.Equals(a, "[diag]", StringComparison.OrdinalIgnoreCase)).ToArray();
            }
#endif

            return new HostBuilder()
                // Use AppContext.BaseDirectory as the content root. We expect "content" to refer to files co-located with the tool
                // So, we don't want to use Directory.GetCurrentDirectory here.
                .UseContentRoot(AppContext.BaseDirectory)
                .ConfigureHostConfiguration(config =>
                {
                    // Use 'DOTNET_' environment variables for host config (this is most a debug concept).
                    config.AddEnvironmentVariables(configEnvironmentVariablePrefix ?? "DOTNET_");

                    // We DON'T make command line args into config, because we're a console tool. We do our own parsing.
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    // Add default filter for our loggers to error or above
                    logging.AddFilter("Contrib", diag ? LogLevel.Debug : LogLevel.Error);
                    logging.SetMinimumLevel(diag ? LogLevel.Debug : LogLevel.Error);

                    // Add config for completion, but we don't really provide a way to load app config...
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                    // TODO: Verbosity from command-line.

                    // Quiet down the hosting lifetime logging.
                    logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);

                    logging.AddConsole();
                    logging.AddDebug();

                    // My spicy thought here is that EventSource logging is *always* desirable.
                    logging.AddEventSourceLogger();
                })
                .ConfigureServices(services =>
                {
                    // Add command-line args
                    services.AddSingleton<ICommandLineAccessor>(new CommandLineAccessor(args, Environment.CommandLine));
                })
                .ConfigureServices(ConfigureServices);
        }

        private static void ConfigureServices(HostBuilderContext hostingContext, IServiceCollection services)
        {
            // Add the default services
            services.AddSingleton<IEntryPoint, DefaultEntryPoint>();
            services.AddSingleton<ICommandDiscoverer, DefaultCommandDiscoverer>();
            services.AddSingleton<IConsole, SystemConsole>();
            services.AddSingleton<IHostLifetime, ToolLifetime>();
        }
    }
}
