using Newtonsoft.Json;

namespace RSSBot
{
    class DiscordEmbedThumbnail : DiscordWebhookObject
    {
        public string url { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }
}
