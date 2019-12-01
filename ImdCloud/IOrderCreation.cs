using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public interface IOrderCreation
    {
        ValueTask<OrderCreationResult> Execute(CancellationToken token);
    }

    public class OrderCreationResult
    {
        public int OrderId { get; set; }
    }
}