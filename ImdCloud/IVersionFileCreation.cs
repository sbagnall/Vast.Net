using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public interface IVersionFileCreation
    {
        ValueTask<int> Execute(int versionId, int fileSize, string fileName, CancellationToken token);
    }
}