using Newtonsoft.Json;

namespace RSSBot
{
    class DiscordEmbedThumbnail
    {
        public string url { get; set; }
        public int height { get; set; }
        public int width { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
