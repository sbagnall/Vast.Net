using System.Threading;
using System.Threading.Tasks;
using static ImdCloud.S3Credentials;

namespace ImdCloud
{
    public interface IS3Credentials
    {
        ValueTask<S3CredentialsResult> Execute(CancellationToken token);
    }
}