using System.Collections.Generic;

namespace Contrib.Extensions.Hosting.Tool
{
    public interface ICommandLineAccessor
    {
        IReadOnlyList<string> Arguments { get; }
        string CommandLine { get; }
    }
}
