using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Kudu.Core.Functions
{
    public class MasterKeyJsonOps : IKeyJsonOps<MasterKey>
    {
        public int GetKeyNumbers()
        {
            return 2;
        }

        public string GenerateKeyJson(Tuple<string,string>[] keyPair, out string unencryptedKey)
        {
            unencryptedKey = keyPair[0].Item1;
            return JObject.Parse($"{{\"masterKey\":{{\"name\":\"master\",\"value\":\"{keyPair[0].Item2}\",\"encrypted\": true }},\"functionKeys\":[{{\"name\": \"default\",\"value\": \"{keyPair[1].Item2}\",\"encrypted\": true}}]}}").ToString(Formatting.Indented);
        }

        public string GetKeyInString(string json, out bool isEncrypted)
        {
            try
            {
                JObject hostJson = JObject.Parse(json);
                if (hostJson["masterKey"]?.Type == JTokenType.String)
                {
                    isEncrypted = false;
                    return hostJson.Value<string>("masterKey");
                }
                else if (hostJson["masterKey"]?.Type == JTokenType.Object && hostJson["functionKeys"]?.Type == JTokenType.Array)
                {
                    JObject keyObject = hostJson.Value<JObject>("masterKey");
                    isEncrypted = keyObject.Value<bool>("encrypted");
                    return keyObject.Value<string>("value");
                }

            }
            catch (JsonException)
            {
                // all parse issue ==> format exception
            }
            throw new FormatException("Invalid secrets file format.");
        }

        public MasterKey GenerateKeyObject(string masterKey, string Name)
        {
            // name is not used
            return new MasterKey { Key = masterKey };
        }
    }
}
