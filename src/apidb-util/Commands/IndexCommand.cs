using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiDb.Model;
using ApiDb.Storage;
using Microsoft.Extensions.Logging;
using Mono.Cecil;

namespace ApiDb.Util.Commands
{
    /// <summary>
    /// A command to index assemblies
    /// </summary>
    public class IndexCommand
    {
        private readonly IConsole _console;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<IndexCommand> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="IndexCommand"/>
        /// </summary>
        /// <param name="console"></param>
        /// <param name="loggerFactory"></param>
        public IndexCommand(IConsole console, ILoggerFactory loggerFactory)
        {
            _console = console;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<IndexCommand>();
        }

        /// <summary>
        /// Indexes the specified assemblies.
        /// </summary>
        /// <param name="args">The assemblies to index.</param>
        /// <param name="catalog">The directory containing the API Catalog.</param>
        /// <param name="cancellationToken">Triggered when Ctrl-C is pressed.</param>
        public async Task<int> ExecuteAsync(string catalog, IEnumerable<string> args, CancellationToken cancellationToken)
        {
            var assemblies = args?.ToList();
            if(assemblies == null || assemblies.Count == 0)
            {
                _logger.LogError("At least one assembly path must be specified!");
                return 1;
            }

            if(string.IsNullOrEmpty(catalog))
            {
                _logger.LogError("Missing required option '--catalog'.");
                return 1;
            }

            using (var storage = new CsvCatalogStorage(catalog))
            {
                var indexWalker = new IndexWalker(_loggerFactory.CreateLogger<IndexWalker>());
                foreach (var asmPath in assemblies)
                {
                    var asm = AssemblyDefinition.ReadAssembly(asmPath);
                    var details = AssemblyDetails.ForAssembly(asm);

                    _logger.LogInformation("Indexing assembly: {Assembly}.", details.Identity);
                    var index = indexWalker.IndexAssembly(asm, details);
                    _logger.LogInformation("Saving index...");
                    await storage.SaveAssemblyAsync(index, cancellationToken);
                    _logger.LogInformation("Indexed assembly.");

                }
            }

            return 0;
        }
    }
}
