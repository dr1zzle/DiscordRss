using System.Collections.Generic;

namespace RSSBot
{
    internal class DiscordWebhookMessage : DiscordWebhookObject 
    {
        public string content { get; set; }
        public string username { get; set; }
        public string avatar_url { get; set; }
        public DiscordMessageEmbed[] embeds { get; set; }

        public DiscordWebhookMessage DeepCopy()
        {
            var returnValue = base.Copy<DiscordWebhookMessage>();
            var embedList = new List<DiscordMessageEmbed>();
            foreach (var embed in embeds)
                embedList.Add(embed.DeepCopy());
            returnValue.embeds = embedList.ToArray();
            return returnValue;
        }
    }
}
