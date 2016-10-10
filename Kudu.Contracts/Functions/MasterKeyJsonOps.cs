using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Kudu.Core.Functions
{
    public class MasterKeyJsonOps : IKeyJsonOps<MasterKey>
    {
        public int RequireKeyCount()
        {
            return 2;
        }

        public string GenerateKeyUglyJson(Tuple<string,string>[] keyPair, out string unencryptedKey)
        {
            unencryptedKey = keyPair[0].Item1;
            return $"{{\"masterKey\":{{\"name\":\"master\",\"value\":\"{keyPair[0].Item2}\",\"encrypted\": true }},\"functionKeys\":[{{\"name\": \"default\",\"value\": \"{keyPair[1].Item2}\",\"encrypted\": true}}]}}";
        }

        public static int GetVersion(JObject hostJson)
        {
            if (hostJson["masterKey"]?.Type == JTokenType.String && hostJson["functionKey"]?.Type == JTokenType.String)
            {
                return 0;
            }
            else if (hostJson["masterKey"]?.Type == JTokenType.Object && hostJson["functionKeys"]?.Type == JTokenType.Array)
            {
                return 1;
            }
            return -1;
        }

        public string GetKeyInString(string json, out bool isEncrypted)
        {
            try
            {
                JObject hostJson = JObject.Parse(json);
                switch (GetVersion(hostJson))
                {
                    case 0:
                        isEncrypted = false;
                        return hostJson.Value<string>("masterKey");
                    case 1:
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
