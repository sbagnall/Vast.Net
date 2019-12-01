using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public class VersionCreation : IVersionCreation
    {
        public ValueTask<VersionCreationResult> Execute(int orderId, CancellationToken token)
        {
            // TODO: implement 

            return new ValueTask<VersionCreationResult>(new VersionCreationResult()
            {
                Id = 1
            });
        }
    }
}