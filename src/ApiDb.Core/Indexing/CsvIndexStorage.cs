using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ApiDb.Model;

namespace ApiDb.Indexing
{
    public class CsvIndexStorage : IndexStorage
    {
        private readonly string _rootDirectory;

        public CsvIndexStorage(string rootDirectory)
        {
            _rootDirectory = rootDirectory;
        }

        public override async Task SaveAssemblyAsync(AssemblyIndex index, CancellationToken cancellationToken = default)
        {
            var asmDir = Path.Combine(_rootDirectory, index.Details.Identity.UniqueId.Substring(0, 2), index.Details.Identity.UniqueId);
            if (!Directory.Exists(asmDir))
            {
                Directory.CreateDirectory(asmDir);
            }

            await Task.WhenAll(
                SaveDetailsAsync(Path.Combine(asmDir, $"{index.Details.Identity.AssemblyName.Name}.txt"), index.Details, cancellationToken),
                SaveApisAsync(Path.Combine(asmDir, "apis.csv"), index.ApiReferences, cancellationToken));
        }

        private async Task SaveApisAsync(string fileName, IEnumerable<ApiReference> apiReferences, CancellationToken cancellationToken)
        {
            using (var writer = new StreamWriter(fileName))
            {
                await writer.WriteLineAsync("Kind,AssemblyName,AssemblyId,Source,Target");
                foreach(var reference in apiReferences)
                {
                    await writer.WriteFieldsAsync(
                        reference.Kind.ToString(),
                        reference.Assembly.AssemblyName.Name,
                        reference.Assembly.ToString(),
                        reference.Source.ToString(),
                        reference.Target.ToString());
                }
            }
        }

        private async Task SaveDetailsAsync(string fileName, AssemblyDetails details, CancellationToken cancellationToken)
        {
            using (var writer = new StreamWriter(fileName))
            {
                await writer.WriteLineAsync($"AssemblyId: {details.Identity}");
                await writer.WriteLineAsync($"FullName: {details.Identity.AssemblyName.FullName}");
                await writer.WriteLineAsync($"InformationalVersion: {details.InformationalVersion}");

                if (details.Build != null)
                {
                    await writer.WriteLineAsync($"Repository: {details.Build.Repository}");
                    await writer.WriteLineAsync($"SourceVersion: {details.Build.SourceVersion}");
                    await writer.WriteLineAsync($"SourceUrl: {details.Build.SourceUrl}");
                    await writer.WriteLineAsync($"Branch: {details.Build.Branch}");
                    await writer.WriteLineAsync($"BuildName: {details.Build.BuildName}");
                    await writer.WriteLineAsync($"BuildNumber: {details.Build.BuildNumber}");
                }
            }
        }
    }
}
