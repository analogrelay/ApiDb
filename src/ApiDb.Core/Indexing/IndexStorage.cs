using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiDb.Model;

namespace ApiDb.Indexing
{
    public abstract class IndexStorage: IDisposable
    {
        public virtual void Dispose()
        {
        }

        public abstract Task SaveReferencesAsync(IReadOnlyList<ApiReference> references);
    }
}
