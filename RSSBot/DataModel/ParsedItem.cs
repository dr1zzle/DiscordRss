using RSSBot.DiscordWebhookModels;

namespace RSSBot.DataModel
{
    public class ParsedItem
    {
        public string Url { get; set; }
        public DiscordWebhookMessage ParsedMessage { get; set; }

        public ParsedItem(string url, DiscordWebhookMessage parsedMessage)
        {
            Url = url;
            ParsedMessage = parsedMessage;
        }
    }
}
