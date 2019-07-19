using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ApiDb.Indexing;
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

        /// <summary>
        /// Creates a new instance of <see cref="IndexCommand"/>
        /// </summary>
        /// <param name="console"></param>
        /// <param name="loggerFactory"></param>
        public IndexCommand(IConsole console, ILoggerFactory loggerFactory)
        {
            _console = console;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Indexes the specified assemblies.
        /// </summary>
        /// <param name="args">The assemblies to index.</param>
        /// <param name="outputFile">The file to write the index to.</param>
        /// <param name="cancellationToken">Triggered when Ctrl-C is pressed.</param>
        public async Task<int> ExecuteAsync(string outputFile, IEnumerable<string> args, CancellationToken cancellationToken)
        {
            using (var storage = CreateStorage(outputFile))
            {
                var indexWalker = new IndexWalker(_loggerFactory.CreateLogger<IndexWalker>());
                foreach (var asmPath in args)
                {
                    var asm = AssemblyDefinition.ReadAssembly(asmPath);
                    indexWalker.AddAssembly(asm);
                }

                await storage.SaveReferencesAsync(indexWalker.IndexedApis);
            }

            return 0;
        }

        private IndexStorage CreateStorage(string outputFile)
        {
            if(string.IsNullOrEmpty(outputFile))
            {
                return new InMemoryIndexStorage();
            }
            else
            {
                return new FlatFileIndexStorage(new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None));
            }
        }
    }
}
