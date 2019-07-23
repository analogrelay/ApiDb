using System.Threading;
using System.Threading.Tasks;
using ApiDb.Model;

namespace ApiDb.Indexing
{
    public class InMemoryIndexStorage : IndexStorage
    {
        public override Task SaveAssemblyAsync(AssemblyIndex index, CancellationToken cancellationToken = default)
        {
            if (index is null)
            {
                throw new System.ArgumentNullException(nameof(index));
            }

            return Task.CompletedTask;
        }
    }
}
