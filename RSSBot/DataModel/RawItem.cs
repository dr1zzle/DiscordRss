using System.Xml.Linq;

using RSSBot.Configuration.ConfigModels;

namespace RSSBot.DataModel
{
    internal class RawItem
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RawItem"/>.
        /// </summary>
        public RawItem() { }

        /// <summary>
        /// Initializes a new instance of <see cref="RawItem"/>.
        /// <param name="entity">RssWebhookEntity wich belongs to the message.</param>
        /// <param name="rawMessage">Raw message as a XElement.</param>
        /// </summary>
        public RawItem(XElement rawMessage, RssWebhookEntity entity)
        {
            Entity = entity;
            RawMessage = rawMessage;
        }

        public XElement RawMessage { get; set; }
        public RssWebhookEntity Entity { get; set; }
    }
}
