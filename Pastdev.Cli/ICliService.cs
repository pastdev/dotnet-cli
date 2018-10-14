using System.Threading;
using System.Threading.Tasks;

namespace Pastdev.Cli
{
    public interface ICliService
    {
        Task<int> RunAsync(CancellationToken cancellationToken);
    }
}
