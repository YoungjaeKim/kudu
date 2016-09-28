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
                    return false;
                }
                else if (hostJson["masterKey"]?.Type == JTokenType.Object)
                {
                    key = hostJson.Value<JObject>("masterKey").Value<string>("value");
                    return true;
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
