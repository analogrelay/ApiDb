using System.Threading.Tasks;

namespace Contrib.Extensions.Hosting.Tool
{
    public interface IEntryPoint
    {
        Task<int> ExecuteAsync();
    }
}