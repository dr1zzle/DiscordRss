using System.Collections.Generic;

namespace RSSBot
{
    internal class RssWebhookEntity
    {
        public RssWebhookEntity()
        {
            LastFeeds = new List<string>();
        }

        public string Url { get; set; }

        public List<string> LastFeeds { get; set; }

        public DiscordWebhookMessage WebhookMessageTemplate { get; set; }
    }
}
