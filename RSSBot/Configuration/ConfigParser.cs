using System;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RSSBot.Configuration.ConfigModels;
using RSSBot.Logging;

namespace RSSBot.Configuration
{
    public class ConfigParser
    {
        private readonly ILogger _logger;
        private readonly string _feedsLocation;
        private readonly string _configLocation;

        public ConfigParser(ILogger logger, string feedsLocation, string configLocation)
        {
            _logger = logger;
            _feedsLocation = feedsLocation;
            _configLocation = configLocation;
        }

        public Feeds GetFeeds() => ReadJsonFile(_feedsLocation).ToObject<Feeds>();

        public Config GetConfig() => ReadJsonFile(_configLocation).ToObject<Config>();

        private JObject ReadJsonFile(string path)
        {
            try
            {
                JObject returnValue = null;
                using (var file = File.OpenText(path))
                {
                    using (var jsonReader = new JsonTextReader(file))
                    {
                        returnValue = (JObject)JToken.ReadFrom(jsonReader);
                    }
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
