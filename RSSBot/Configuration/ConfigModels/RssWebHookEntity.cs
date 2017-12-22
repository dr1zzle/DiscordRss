using System.Collections.Generic;

using Newtonsoft.Json;

using RSSBot.DataModel;
using RSSBot.DiscordWebhookModels;

namespace RSSBot.Configuration.ConfigModels
{
    internal class RssWebhookEntity
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RssWebhookEntity"/> & and delcares <see cref="StoredFeeds"/>.
        /// </summary>
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
