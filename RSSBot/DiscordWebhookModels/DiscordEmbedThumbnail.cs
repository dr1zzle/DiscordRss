using Newtonsoft.Json;

namespace RSSBot.DiscordWebhookModels
{
    internal class DiscordEmbedThumbnail : DiscordWebhookObject
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("width")]
        public int width { get; set; }
    }
}
