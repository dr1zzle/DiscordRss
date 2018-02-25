using System.Collections.Generic;

using Newtonsoft.Json;

namespace RSSBot.Configuration.ConfigModels
{
    public class Feeds
    {
        [JsonProperty("feeds")]
        public IList<RssWebhookEntity> FeedList { get; set; }
    }
}
