using Newtonsoft.Json.Linq;
using System;

namespace Kudu.Core.Functions
{
    public interface IKeyOperation
    {
        JObject GenerateKeyJson(string functionKey);
        // return IsEncrypted
        bool GetKeyInString(string json, out string key);

        Object ReturnKeyObject(string functionKey, string functionName);
    }
}
