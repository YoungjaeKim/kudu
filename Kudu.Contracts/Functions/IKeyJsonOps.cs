using Newtonsoft.Json.Linq;
using System;

namespace Kudu.Core.Functions
{
    public interface IKeyJsonOps<T>
    {
        int RequireKeyCount();

        string GenerateKeyUglyJson(Tuple<string,string>[] keyPairs, out string unencryptedKey);

        string GetKeyInString(string json, out bool isEncrypted);

        T GenerateKeyObject(string functionKey, string functionName);
    }
}
