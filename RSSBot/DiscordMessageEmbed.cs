using Newtonsoft.Json;

namespace RSSBot
{
    class DiscordMessageEmbed
    {
        public string title { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public DiscordEmbedThumbnail thumbnail { get; set; }
        public int color { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
