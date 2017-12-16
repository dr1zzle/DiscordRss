using Newtonsoft.Json;

namespace RSSBot
{
    internal class DiscordMessageEmbed : DiscordWebhookObject
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("thumbnail")]
        public DiscordEmbedThumbnail Thumbnail { get; set; }
        [JsonProperty("color")]
        public int Color { get; set; }

        public DiscordMessageEmbed DeepCopy() 
        {
            var returnValue = base.Copy<DiscordMessageEmbed>();
            returnValue.Thumbnail = Thumbnail.Copy<DiscordEmbedThumbnail>();
            return returnValue;
        }
    }
}
