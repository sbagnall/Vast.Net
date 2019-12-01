using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public interface IOrderCreation
    {
        ValueTask<OrderCreationResult> Execute(CancellationToken token);
    }
}