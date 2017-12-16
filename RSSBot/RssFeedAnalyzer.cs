using System.Net;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using RSSBot.Configuration.ConfigModels;

using RSSBot.DataModel;
using System;

namespace RSSBot
{
    internal class RssFeedAnalyzer
    {
        private Client HttpClient;
        private XNamespace XNamespace;
        private IList<RssWebhookEntity> RssWebhookEntities;

        public RssFeedAnalyzer(Feeds feeds, Client httpClient)
        {
            HttpClient = httpClient;
            RssWebhookEntities = feeds.FeedList;
            XNamespace = "http://search.yahoo.com/mrss/";
        }

        public async Task<IList<KeyValuePair<string, DiscordWebhookMessage>>> GetRssMessagesToSend()
        {
            var msgsToSend = await GetNewRssFeeds();
            var returnList = new List<KeyValuePair<string, DiscordWebhookMessage>>();
            foreach (var msg in msgsToSend)
            {
                var setupMsg = RssWebhookEntities.First(x => x.Url == msg.Key).WebhookMessageTemplate.DeepCopy();
                setupMsg.Embeds.First().Url = msg.Value.Element("link").Value;
                setupMsg.Embeds.First().Title = msg.Value.Element("title").Value;
                var media = msg.Value.Element(XNamespace + "content");
                if (media != null)
                {
                    var thumbnail = media.Elements(XNamespace + "thumbnail").FirstOrDefault();
                    if (thumbnail != null)
                        setupMsg.Embeds.First().Thumbnail = new DiscordEmbedThumbnail
                        {
                            Height = 50,
                            width = 50,
                            Url = thumbnail.Attribute("url").Value
                        };
                }
                setupMsg.Embeds.First().Description = WebUtility.HtmlDecode(Regex.Replace(msg.Value.Element("description")
                    .Value.Replace("\n", string.Empty)
                    .Replace("\t", string.Empty), "<.*?>", string.Empty));
                returnList.Add(new KeyValuePair<string, DiscordWebhookMessage>(RssWebhookEntities.First(x => x.Url == msg.Key).Webhook, setupMsg));
            }
            return returnList;
        }

        private async Task<IList<KeyValuePair<string, XElement>>> GetNewRssFeeds()
        {
            var msgsToSend = new List<KeyValuePair<string, XElement>>();
            foreach (var entity in RssWebhookEntities)
            {
                var store = entity.StoredFeeds;
                var items = await HttpClient.TryGetFeeds(entity.Url);
                if (store.Count != 0)
                {
                    foreach (var item in items)
                    {
                        var linkValue = item.Element("link").Value;
                        if (store.Count(x => x.Id == linkValue) == 0)
                        {
                            msgsToSend.Add(new KeyValuePair<string, XElement>(entity.Url, item));
                            store.Add(ParseRssFeed(item));
                            if (store.Count > 150)
                                store.RemoveAt(0);                                
                        }
                    }
                }
                else
                {
                    entity.StoredFeeds = items.Select(x => ParseRssFeed(x)).ToList();
                }
            }
            return msgsToSend;
        }

        private FeedInfoItem ParseRssFeed(XElement rssFeedElement) =>
            new FeedInfoItem(
            rssFeedElement.Element("link").Value,
            rssFeedElement.Element("title").Value,
            DateTime.Parse(rssFeedElement.Element("pubDate").Value));
    }
}
