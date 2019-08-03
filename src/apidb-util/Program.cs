using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiDb.Util.Commands;
using ApiDb.Util.Commands.Kusto;
using Contrib.Extensions.Hosting.Tool;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiDb.Util
{
    internal class Program
    {
        static Task Main(string[] args) =>
            ToolHost.CreateDefaultBuilder(args, configEnvironmentVariablePrefix: "APIDB_")
                .ConfigureLogging(logging =>
                {
                    // TODO: Get from args.
                    logging.AddFilter("ApiDb", LogLevel.Information);
                })
                .RunToolAsync();

        public static IEnumerable<Type> GetSubcommands()
        {
            yield return typeof(IndexCommand);
            yield return typeof(LoginCommand);
            yield return typeof(TokensCommand);
            yield return typeof(KustoCommand);
        }
    }
}
