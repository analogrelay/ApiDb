using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ApiDb.Util.Commands
{
    internal class LoginCommand
    {
        private readonly IConsole _console;
        private readonly ILogger<LoginCommand> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="LoginCommand"/>
        /// </summary>
        /// <param name="console"></param>
        /// <param name="logger"></param>
        public LoginCommand(IConsole console, ILogger<LoginCommand> logger)
        {
            _console = console;
            _logger = logger;
        }

        /// <summary>
        /// Acquires an access token for the specified cluster and stores it in the cache.
        /// </summary>
        /// <param name="cluster">The cluster to acquire an access token for.</param>
        /// <param name="cancellationToken">Triggered when Ctrl-C is pressed.</param>
        public async Task<int> ExecuteAsync(string cluster, CancellationToken cancellationToken)
        {
            if(!cluster.EndsWith('/'))
            {
                cluster = $"{cluster}/";
            }

            var result = await AuthenticationHelper.AcquireNewTokenAsync(new[]
            {
                $"{cluster}/.default"
            }, _logger, cancellationToken);
            _logger.LogInformation("Acquired access token for {Cluster}.", cluster);
            return 0;
        }
    }
}
