using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Contrib.Extensions.Hosting.Tool
{
    internal class DefaultEntryPoint : IEntryPoint
    {
        private readonly ICommandLineAccessor _commandLineAccessor;
        private readonly ICommandDiscoverer _discoverer;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger<DefaultEntryPoint> _logger;

        public DefaultEntryPoint(ICommandLineAccessor commandLineAccessor, ICommandDiscoverer discoverer, IHostApplicationLifetime applicationLifetime, ILogger<DefaultEntryPoint> logger)
        {
            _commandLineAccessor = commandLineAccessor;
            _discoverer = discoverer;
            _applicationLifetime = applicationLifetime;
            _logger = logger;
        }

        public async Task<int> ExecuteAsync()
        {
            // Discover commands
            _logger.LogDebug("Discovering commands...");
            var builder = _discoverer.CreateCommandLineBuilder();
            _logger.LogInformation("Discovered {CommandName} command.", builder.Command.Name);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                // Dump the parsed command info
                DumpSymbol(builder.Command);
            }

            builder.UseMiddleware((context, next) =>
            {
                context.BindingContext.AddService(typeof(CancellationToken), () => _applicationLifetime.ApplicationStopping);
                return next(context);
            });
            builder.RegisterWithDotnetSuggest();
            builder.UseSuggestDirective();
            builder.UseExceptionHandler((ex, context) =>
            {
                context.InvocationResult = new ErrorResult(ex, _logger);
            });
            builder.UseDebugDirective();
            builder.UseHelp();

            _logger.LogDebug("Parsing command line...");
            var parser = builder.Build();

            _logger.LogDebug("Invoking tool");
            var result = await parser.InvokeAsync(_commandLineAccessor.Arguments.ToArray());
            _logger.LogDebug("Tool exited with exit code '{ExitCode}'", result);

            return result;
        }

        private void DumpSymbol(Symbol symbol, int level = 0)
        {
            switch(symbol)
            {
                case Command cmd:
                    _logger.LogDebug("[{Level}] Command: {Name}", level, cmd.Name);

                    foreach(var subsymbol in cmd)
                    {
                        DumpSymbol(subsymbol, level + 1);
                    }
                    break;
                case Option opt:
                    _logger.LogDebug("[{Level}] Option: {Name} ({Aliases})", level, opt.Name, opt.Aliases);
                    DumpSymbol(opt.Argument, level + 1);
                    break;
                case Argument arg:
                    _logger.LogDebug("[{Level}] Argument: {Type}", level, arg.ArgumentType);
                    break;
            }
        }
    }
}