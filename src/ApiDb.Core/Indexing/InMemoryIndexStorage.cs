using System.Collections.Generic;
using System.Threading.Tasks;
using ApiDb.Model;

namespace ApiDb.Indexing
{
    public class InMemoryIndexStorage : IndexStorage
    {
        private List<ApiReference> _apiReferences = new List<ApiReference>();

        public override Task SaveReferencesAsync(IReadOnlyList<ApiReference> references)
        {
            _apiReferences.AddRange(references);
            return Task.CompletedTask;
        }
    }
}
