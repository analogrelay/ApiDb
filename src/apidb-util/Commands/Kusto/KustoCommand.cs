using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ApiDb.Util.Commands.Kusto
{
    [Description("Commands to manage Kusto-based API Catalog data.")]
    internal class KustoCommand
    {
        public static IEnumerable<Type> GetSubcommands()
        {
            yield return typeof(InitCommand);
        }
    }
}
