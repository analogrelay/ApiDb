using System.IO;
using System.Threading.Tasks;

namespace ApiDb
{
    internal static class TextWriterExtensions
    {
        public static async Task WriteFieldsAsync(this TextWriter writer, params string[] fields)
        {
            for (var i = 0; i < fields.Length; i++)
            {
                if (i != 0)
                {
                    await writer.WriteAsync(",");
                }
                var escaped = fields[i].Replace("\"", "\\\"");
                await writer.WriteAsync($"\"{escaped}\"");
            }
            await writer.WriteLineAsync();
        }
    }
}
