using System.Collections.Generic;

using Newtonsoft.Json;

using RSSBot.DataModel;
using RSSBot.DiscordWebhookModels;

namespace RSSBot.Configuration.ConfigModels
{
    public class RssWebhookEntity
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("webhook")]
        public string Webhook { get; set; }
        [JsonProperty("template")]
        public DiscordWebhookMessage WebhookMessageTemplate { get; set; }

        [JsonIgnore]
        public IList<FeedInfoItem> StoredFeeds { get; set; }

        public RssWebhookEntity()
        {
            StoredFeeds = new List<FeedInfoItem>();
        }
    }
}
