using RSSBot.DiscordWebhookModels;

namespace RSSBot.DataModel
{
    internal class ParsedItem
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ParsedItem"/>.
        /// </summary>
        public ParsedItem() { }

        /// <summary>
        /// Initializes a new instance of <see cref="ParsedItem"/>.
        /// <param name="url">Webhook url.</param>
        /// <param name="parsedMessage">Parsed message of the raw item XElement.</param>
        /// </summary>
        public ParsedItem(string url, DiscordWebhookMessage parsedMessage)
        {
            Url = url;
            ParsedMessage = parsedMessage;
        }

        public string Url { get; set; }
        public DiscordWebhookMessage ParsedMessage { get; set; }
    }
}
