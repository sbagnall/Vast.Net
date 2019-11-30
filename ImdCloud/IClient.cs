using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public interface IClient
    {
        ValueTask<IDictionary<string, string>> Get(string path, IDictionary<string, string> @params, string backgroundUserToken = default, CancellationToken token = default);

        ValueTask<IDictionary<string, string>> Post(string path, IDictionary<string, string> payload, string backgroundUserToken = default, CancellationToken token = default);
    }
}