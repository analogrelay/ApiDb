using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiDb.Model;

namespace ApiDb.Indexing
{
    public abstract class IndexStorage: IDisposable
    {
        public virtual void Dispose()
        {
        }

        public abstract Task SaveAssemblyAsync(AssemblyIndex index, CancellationToken cancellationToken = default);
    }
}
