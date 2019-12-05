using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public interface IClient
    {
        ValueTask<T> Get<T>(
            string path, 
            string backgroundUserToken, 
            CancellationToken token,
            IDictionary<string, string> @params = default) where T: new();

        ValueTask<T> Post<T>(
            string path,
            JObject payload,
            string backgroundUserToken,
            CancellationToken token) where T: new();
    }
}