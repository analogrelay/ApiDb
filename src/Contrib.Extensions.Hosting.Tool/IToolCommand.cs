using System.CommandLine;

namespace Contrib.Extensions.Hosting.Tool
{
    public interface IToolCommand
    {
        Command BuildCommand();
    }
}