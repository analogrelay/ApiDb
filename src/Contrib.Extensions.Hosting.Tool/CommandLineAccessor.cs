using System.Collections.Generic;

namespace Contrib.Extensions.Hosting.Tool
{
    public class CommandLineAccessor : ICommandLineAccessor
    {
        public CommandLineAccessor(IReadOnlyList<string> arguments, string commandLine)
        {
            Arguments = arguments;
            CommandLine = commandLine;
        }

        public IReadOnlyList<string> Arguments { get; }
        public string CommandLine { get; }
    }
}
