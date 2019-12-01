using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public interface IIngestMedia
    {
        ValueTask<string> GeneratePreSignedURL(int fileSize, string fileName, CancellationToken token);
    }
}