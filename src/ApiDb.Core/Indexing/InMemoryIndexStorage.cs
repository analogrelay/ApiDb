using System.Threading;
using System.Threading.Tasks;
using ApiDb.Model;

namespace ApiDb.Indexing
{
    public class InMemoryIndexStorage : IndexStorage
    {
        public override Task SaveAssemblyAsync(AssemblyIndex index, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
