using Newtonsoft.Json;

namespace RSSBot.Configuration.ConfigModels
{
    internal class Config
    {
        [JsonProperty("cycleTime")]
        public int CycleTime { get; set; }
    }
}
