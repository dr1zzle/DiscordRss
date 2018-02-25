using Newtonsoft.Json;

namespace RSSBot.Configuration.ConfigModels
{
    public class Config
    {
        [JsonProperty("cycleTime")]
        public int CycleTime { get; set; }
    }
}
