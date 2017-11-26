namespace RSSBot
{
    internal class DiscordEmbedThumbnail : DiscordWebhookObject
    {
        public string url { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }
}
