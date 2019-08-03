using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ApiDb.Util.Commands.Kusto
{
    internal class InitCommand
    {
        private readonly IConsole _console;
        private readonly ILogger<InitCommand> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="InitCommand"/>
        /// </summary>
        /// <param name="console"></param>
        /// <param name="logger"></param>
        public InitCommand(IConsole console, ILogger<InitCommand> logger)
        {
            _console = console;
            _logger = logger;
        }

        /// <summary>
        /// Initializes the API DB schema in the specified Kusto Database
        /// </summary>
        /// <param name="cluster">The cluster containing the database to initialize.</param>
        /// <param name="database">The name of the database to initialize.</param>
        /// <param name="cancellationToken">Triggered when Ctrl-C is pressed.</param>
        public async Task<int> ExecuteAsync(string cluster, string database, CancellationToken cancellationToken)
        {
            _console.Out.WriteLine("Initializing");
            return 0;
        }
    }
}