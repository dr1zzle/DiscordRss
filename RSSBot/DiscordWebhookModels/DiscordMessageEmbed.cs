namespace RSSBot
{
    internal class DiscordMessageEmbed : DiscordWebhookObject
    {
        public string title { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public DiscordEmbedThumbnail thumbnail { get; set; }
        public int color { get; set; }

        public DiscordMessageEmbed DeepCopy() 
        {
            var returnValue = base.Copy<DiscordMessageEmbed>();
            returnValue.thumbnail = thumbnail.Copy<DiscordEmbedThumbnail>();
            return returnValue;
        }
    }
}
