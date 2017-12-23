using System;
using System.Net;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using RSSBot.Configuration.ConfigModels;

using RSSBot.DataModel;
using RSSBot.DiscordWebhookModels;

namespace RSSBot
{
    internal class RssFeedAnalyzer
    {
        private Client HttpClient;
        private XNamespace XNamespace;
        private IList<RssWebhookEntity> RssWebhookEntities;

        /// <summary>
        /// Initializes a new instance of <see cref="RssFeedAnalyzer"/>.
        /// </summary>
        /// <param name="feeds">Feeds given by the config file: feeds.json.</param>
        /// <param name="httpClient">HttpClient to perform get request on rss feeds.</param>
        public RssFeedAnalyzer(Feeds feeds, Client httpClient)
        {
            HttpClient = httpClient;
            RssWebhookEntities = feeds.FeedList;
            XNamespace = "http://search.yahoo.com/mrss/";
        }

        public async Task<IList<ParsedItem>> GetRssMessagesToSend()
        {
            var msgsToSend = await GetNewRssFeeds();
            var returnList = new List<ParsedItem>();
            foreach (var msg in msgsToSend)
            {
                var entity = msg.Entity;
                var raw = msg.RawMessage;
                var setupMsg = entity.WebhookMessageTemplate.DeepCopy();
                var embed = setupMsg.Embeds.First();

                embed.Url = raw.Element("link").Value;
                embed.Title = raw.Element("title").Value;
                var media = raw.Element(XNamespace + "content");
                if (media != null)
                {
                    var thumbnail = media.Elements(XNamespace + "thumbnail").FirstOrDefault();
                    if (thumbnail != null)
                        embed.Thumbnail = new DiscordEmbedThumbnail
                        {
                            Height = 50,
                            width = 50,
                            Url = thumbnail.Attribute("url").Value
                        };
                }
                var description = WebUtility.HtmlDecode(Regex.Replace(raw.Element("description")
                    .Value.Replace("\n", string.Empty)
                    .Replace("\t", string.Empty), "<.*?>", string.Empty));
                embed.Description = description.Length > 300 ? description.Substring(0, 300) + "[...]" : description;
                returnList.Add(new ParsedItem(entity.Webhook, setupMsg));
            }
            return returnList;
        }

        private async Task<IList<RawItem>> GetNewRssFeeds()
        {
            var msgsToSend = new List<RawItem>();
            foreach (var entity in RssWebhookEntities)
            {
                var store = entity.StoredFeeds;
                var items = await HttpClient.TryGetFeeds(entity.Url);
                if (store.Count == 0)
                {
                    entity.StoredFeeds = items.Select(x => ParseRssFeed(x)).ToList();
                    continue;
                }
                foreach (var item in items)
                {
                    if (store.Count(x => x.Id == item.Element("link").Value) == 0)
                    {
                        msgsToSend.Add(new RawItem(item, entity));
                        store.Add(ParseRssFeed(item));
                        if (store.Count > 150)
                            store.RemoveAt(0);
                    }
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
