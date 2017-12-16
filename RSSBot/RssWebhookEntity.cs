using System.Collections.Generic;

namespace RSSBot
{
    internal class RssWebhookEntity
    {
        public RssWebhookEntity()
        {
            LastFeeds = new List<string>();
            SentFeeds = new List<string>();
        }

        public string Url { get; set; }
        public string Webhook { get; set; }
        public IList<string> LastFeeds { get; set; }
        public IList<string> SentFeeds { get; set; }
        public DiscordWebhookMessage WebhookMessageTemplate { get; set; }
    }
}
