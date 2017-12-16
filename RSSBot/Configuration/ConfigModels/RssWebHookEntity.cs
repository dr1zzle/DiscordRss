using System.Collections.Generic;

using Newtonsoft.Json;

using RSSBot.DataModel;

namespace RSSBot.Configuration.ConfigModels
{
    internal class RssWebhookEntity
    {
        public RssWebhookEntity()
        {
            StoredFeeds = new List<FeedInfoItem>();
        }

        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("webhook")]
        public string Webhook { get; set; }
        [JsonProperty("template")]
        public DiscordWebhookMessage WebhookMessageTemplate { get; set; }

        [JsonIgnore]
        public IList<FeedInfoItem> StoredFeeds { get; set; }
    }
}
