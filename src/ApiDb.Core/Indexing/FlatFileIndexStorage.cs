using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ApiDb.Model;

namespace ApiDb.Indexing
{
    public class FlatFileIndexStorage : IndexStorage
    {
        private readonly Stream _file;

        public FlatFileIndexStorage(Stream file)
        {
            _file = file;
        }

        public override async Task SaveReferencesAsync(IReadOnlyList<ApiReference> references)
        {
            using (var writer = new StreamWriter(_file, Encoding.UTF8, bufferSize: 4096, leaveOpen: true))
            {
                foreach(var reference in references)
                {
                    await writer.WriteLineAsync(reference.ToString());
                }
            }
        }

        public override void Dispose()
        {
            _file.Dispose();
        }
    }
}
