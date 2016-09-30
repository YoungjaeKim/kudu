using Newtonsoft.Json.Linq;
using System;

namespace Kudu.Core.Functions
{
    public interface IKeyJsonOps<T>
    {
        int GetKeyNumbers();
        string GenerateKeyJson(Tuple<string,string>[] keyPairs, out string unencryptedKey);
        // return IsEncrypted
        string GetKeyInString(string json, out bool isEncrypted);

        T GenerateKeyObject(string functionKey, string functionName);
    }
}
