using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RSSBot.Configuration.ConfigModels;

namespace RSSBot.Configuration
{
    internal class ConfigParser
    {
        public Feeds GetFeeds() => ReadJsonFile("Configuration/feeds.json").ToObject<Feeds>();

        public Config GetConfig() => ReadJsonFile("Configuration/config.json").ToObject<Config>();

        private JObject ReadJsonFile(string path)
        {
            JObject returnValue = null;
            using (var file = File.OpenText(path))
            using (var jsonReader = new JsonTextReader(file))
                returnValue = (JObject)JToken.ReadFrom(jsonReader);
            return returnValue;
        }
    }
}
