using System.Xml.Linq;

using RSSBot.Configuration.ConfigModels;

namespace RSSBot.DataModel
{
    public class RawItem
    {
        public XElement RawMessage { get; set; }
        public RssWebhookEntity Entity { get; set; }

        public RawItem(XElement rawMessage, RssWebhookEntity entity)
        {
            Entity = entity;
            RawMessage = rawMessage;
        }
    }
}
