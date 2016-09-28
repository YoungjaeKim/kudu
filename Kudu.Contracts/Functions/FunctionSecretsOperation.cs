using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Kudu.Core.Functions
{
    public class FunctionSecretsOperation : IKeyOperation
    {
        // have the schema related info enclosed in this class
        public JObject GenerateKeyJson(string functionKey)
        {
            JObject hostJson = JObject.Parse($"{{\"keys\":[{{\"name\":\"default\",\"value\":\"{functionKey}\",\"encrypted\": true }}]}}");
            return hostJson;
        }

        public bool GetKeyInString(string json, out string key)
        {
            try
            {
                key = null;
                JObject hostJson = JObject.Parse(json);
                if (hostJson["key"]?.Type == JTokenType.String)
                {
                    key = hostJson.Value<string>("key");
                    return false;
                }
                else if (hostJson["keys"]?.Type == JTokenType.Array)
                {
                    JArray keys = hostJson.Value<JArray>("keys");
                    if (keys.Count >= 1)
                    {
                        key = keys[0].Value<string>("value");
                        for (int i = 1; i < keys.Count; i++)
                        {
                            if (String.Equals(keys[i].Value<string>("name"), "default"))
                            {
                                key = keys[i].Value<string>("value");
                            }
                        }
                        return true;
                    }
                }
            }
            catch (JsonException)
            {
                // all parse issue ==> format exception
                throw new FormatException("Invalid secrets file format.");
            }
            return false;
        }

        public Object ReturnKeyObject(string functionKey, string functionName)
        {
            // cast later
            return JsonConvert.DeserializeObject<FunctionSecrets>($"{{\"key\":\"{functionKey}\",\"trigger_url\":\"{String.Format(@"https://{0}/api/{1}?code={2}", System.Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME") ?? "localhost", functionName, functionKey)}\"}}");
        }
    }
}
