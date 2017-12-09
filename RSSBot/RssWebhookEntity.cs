using System.Collections.Generic;

namespace RSSBot
{
    internal class RssWebhookEntity
    {
        public string Url { get; set; }
        public string Webhook { get; set; }
        public IList<string> LastFeeds { get; set; } = new List<string>();
        public IList<string> SentFeeds { get; set; } = new List<string>();
        public DiscordWebhookMessage WebhookMessageTemplate { get; set; }
    }
}
