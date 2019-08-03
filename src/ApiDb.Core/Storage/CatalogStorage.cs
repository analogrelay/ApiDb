using System;
using System.Threading;
using System.Threading.Tasks;
using ApiDb.Model;

namespace ApiDb.Storage
{
    public abstract class CatalogStorage: IDisposable
    {
        public virtual void Dispose()
        {
        }

        public abstract Task SaveAssemblyAsync(AssemblyIndex index, CancellationToken cancellationToken = default);
    }
}
