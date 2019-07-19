using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Contrib.Extensions.Hosting.Tool
{
    public static class ToolHostHostBuilderExtensions
    {
        public static async Task<int> RunToolAsync(this IHostBuilder builder)
        {
            // We MUST dispose the host or the ConsoleLifetime's OnProcessExit will hang.
            // See https://github.com/aspnet/Extensions/issues/1363
            using (var host = builder.Build())
            {
                await host.StartAsync();

                // Get the entrypoint
                var entryPoint = host.Services.GetRequiredService<IEntryPoint>();

                // Run the app
                var exitCode = await entryPoint.ExecuteAsync();

                // Shut down everything.
                await host.StopAsync();

                return exitCode;
            }
        }
    }
}
