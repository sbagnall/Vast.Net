using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public interface IVersionCreation
    {
        ValueTask<VersionCreationResult> Execute(int orderId, CancellationToken token);
    }

    public class VersionCreationResult
    {
        public int Id { get; set; }

        public string VersionId { get; set; }
    }
}