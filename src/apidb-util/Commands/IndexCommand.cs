using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ApiDb.Indexing;
using ApiDb.Model;
using Microsoft.Extensions.Logging;
using Mono.Cecil;

namespace ApiDb.Util.Commands
{
    /// <summary>
    /// A command to generate ApiDb indexes.
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
        /// <param name="outputDir">The directory to write the index to.</param>
        /// <param name="cancellationToken">Triggered when Ctrl-C is pressed.</param>
        public async Task<int> ExecuteAsync(string outputDir, IEnumerable<string> args, CancellationToken cancellationToken)
        {
            using (var storage = CreateStorage(outputDir))
            {
                var indexWalker = new IndexWalker(_loggerFactory.CreateLogger<IndexWalker>());
                foreach (var asmPath in args)
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

        private IndexStorage CreateStorage(string outputDir)
        {
            if(string.IsNullOrEmpty(outputDir))
            {
                return new InMemoryIndexStorage();
            }
            else
            {
                return new CsvIndexStorage(outputDir);
            }
        }
    }
}
