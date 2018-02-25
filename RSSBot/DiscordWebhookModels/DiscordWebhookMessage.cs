using System.Collections.Generic;

using Newtonsoft.Json;

namespace RSSBot.DiscordWebhookModels
{
    public class DiscordWebhookMessage : DiscordWebhookObject
    {
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }
        [JsonProperty("embeds")]
        public IList<DiscordMessageEmbed> Embeds { get; set; }

        public DiscordWebhookMessage DeepCopy()
        {
            var returnValue = base.Copy<DiscordWebhookMessage>();
            var embedList = new List<DiscordMessageEmbed>();
            foreach (var embed in Embeds)
                embedList.Add(embed.DeepCopy());
            returnValue.Embeds = embedList.ToArray();
            return returnValue;
        }
    }
}
