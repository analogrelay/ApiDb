using System;
using System.Collections.Generic;
using System.CommandLine.Builder;
using System.Text;

namespace Contrib.Extensions.Hosting.Tool
{
    public interface ICommandDiscoverer
    {
        CommandLineBuilder CreateCommandLineBuilder();
    }
}
