using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public class VersionFileCreation : IVersionFileCreation
    {
        public ValueTask<int> Execute(int versionId, int fileSize, string fileName, CancellationToken token)
        {
            // TODO: implement 

            return new ValueTask<int>(1);
        }
    }
}