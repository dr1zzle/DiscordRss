using System;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RSSBot.Configuration.ConfigModels;

namespace RSSBot.Configuration
{
    internal class ConfigParser
    {
        public Feeds GetFeeds() => ReadJsonFile(ConfigLocations.Feeds).ToObject<Feeds>();

        public Config GetConfig() => ReadJsonFile(ConfigLocations.Config).ToObject<Config>();

        private JObject ReadJsonFile(string path)
        {
            try
            {
                JObject returnValue = null;
                using (var file = File.OpenText(path))
                using (var jsonReader = new JsonTextReader(file))
                    returnValue = (JObject)JToken.ReadFrom(jsonReader);
                return returnValue;
            }
            catch (Exception ex)
            {
                Program.WriteToLogFile(LoggingLocations.Start, $"{ex.Message} {DateTime.Now}");
                throw;
            }
        }
    }
}
