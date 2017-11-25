using Newtonsoft.Json;

namespace RSSBot
{
    class DiscordWebhookMessage
    {
        public string content { get; set; }
        public string username { get; set; }
        public string avatar_url { get; set; }
        public DiscordMessageEmbed[] embeds { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
