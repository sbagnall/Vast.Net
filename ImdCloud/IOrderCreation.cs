using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public interface IOrderCreation
    {
        ValueTask<OrderCreationResult> Execute(string userToken, CancellationToken token);
    }

    public class OrderCreationResult
    {
        public int OrderId { get; set; }
    }
}