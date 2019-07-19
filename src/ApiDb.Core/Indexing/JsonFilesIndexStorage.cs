using System.IO;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ApiDb.Model;

namespace ApiDb.Indexing
{
    public class JsonFilesIndexStorage : IndexStorage
    {
        private readonly string _rootDirectory;

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreReadOnlyProperties = false,
        };

        public JsonFilesIndexStorage(string rootDirectory)
        {
            _rootDirectory = rootDirectory;
        }

        public override async Task SaveAssemblyAsync(AssemblyIndex index, CancellationToken cancellationToken = default)
        {
            static async Task SaveObjectAsync<T>(string fileName, T value, CancellationToken token)
            {
                using (var asmDetailsStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    await JsonSerializer.WriteAsync(value, asmDetailsStream, _options, token);
                }
            }

            var asmDir = Path.Combine(_rootDirectory, index.Details.Identity.UniqueId.Substring(0, 2), index.Details.Identity.UniqueId);
            if(!Directory.Exists(asmDir))
            {
                Directory.CreateDirectory(asmDir);
            }

            await Task.WhenAll(
                SaveObjectAsync(Path.Combine(asmDir, $"{index.Details.Identity.AssemblyName.Name}.json"), index.Details, cancellationToken),
                SaveObjectAsync(Path.Combine(asmDir, "apis.json"), index.ApiReferences, cancellationToken));
        }
    }
}
