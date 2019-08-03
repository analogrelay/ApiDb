using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ApiDb.Util.Commands
{
    internal class TokensCommand
    {
        private readonly IConsole _console;
        private readonly ILogger<TokensCommand> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="TokensCommand"/>
        /// </summary>
        /// <param name="console"></param>
        /// <param name="logger"></param>
        public TokensCommand(IConsole console, ILogger<TokensCommand> logger)
        {
            _console = console;
            _logger = logger;
        }

        /// <summary>
        /// Lists available tokens
        /// </summary>
        /// <param name="cancellationToken">Triggered when Ctrl-C is pressed.</param>
        public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
        {
            var accounts = await AuthenticationHelper.GetAccountsAsync(cancellationToken);
            foreach(var account in accounts)
            {
                _console.Out.WriteLine($"Account: {account.Username}");
                _console.Out.WriteLine($"  Environment: {account.Environment}");
                _console.Out.WriteLine($"  HomeAccountId.Identifier: {account.HomeAccountId.Identifier}");
                _console.Out.WriteLine($"  HomeAccountId.ObjectId: {account.HomeAccountId.ObjectId}");
                _console.Out.WriteLine($"  HomeAccountId.TenantId: {account.HomeAccountId.TenantId}");
            }
            return 0;
        }
    }
}
