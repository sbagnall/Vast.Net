using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public class OrderCreation : IOrderCreation
    {
        public ValueTask<OrderCreationResult> Execute(CancellationToken token)
        {
            // TODO: implement

            return new ValueTask<OrderCreationResult>(new OrderCreationResult()
            {
                OrderId = 1
            });
        }
    }
}