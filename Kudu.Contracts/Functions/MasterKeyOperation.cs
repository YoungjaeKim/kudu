using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Kudu.Core.Functions
{
    public class MasterKeyOperation : IKeyOperation
    {
        public JObject GenerateKeyJson(string masterKey)
        {
            JObject hostJson = JObject.Parse($"{{\"masterKey\":{{\"name\":\"master\",\"value\":\"{masterKey}\",\"encrypted\": true }}}}");
            return hostJson;
        }

        public bool GetKeyInString(string json, out string key)
        {
            try
            {
                JObject hostJson = JObject.Parse(json);
                if (hostJson["masterKey"]?.Type == JTokenType.String)
                {
                    key = hostJson.Value<string>("masterKey");
                    return false; // always unencrypted(version 0)
                }
                else if (hostJson["masterKey"]?.Type == JTokenType.Object)
                {
                    JObject keyObject = hostJson.Value<JObject>("masterKey");
                    key = keyObject.Value<string>("value");
                    return keyObject.Value<bool>("encrypted");
                }

            }
            catch (JsonException)
            {
                // all parse issue ==> format exception
            }
            throw new FormatException("Invalid secrets file format.");
        }

        public Object ReturnKeyObject(string masterKey, string Name)
        {
            // name is not used
            return JsonConvert.DeserializeObject<MasterKey>($"{{\"masterKey\":\"{masterKey}\"}}");
        }
    }
}
