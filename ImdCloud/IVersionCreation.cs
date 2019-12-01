using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public interface IVersionCreation
    {
        ValueTask<VersionCreationResult> Execute(int orderId, CancellationToken token);
    }
}