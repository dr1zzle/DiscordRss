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
    public class RssFeedAnalyzer
    {
        private static readonly XNamespace XNamespace = "http://search.yahoo.com/mrss/";

        private readonly Client HttpClient;
        private readonly IList<RssWebhookEntity> RssWebhookEntities;

        /// <summary>
        /// Initializes a new instance of <see cref="RssFeedAnalyzer"/>.
        /// </summary>
        /// <param name="feeds">Feeds given by the config file: feeds.json.</param>
        /// <param name="httpClient">HttpClient to perform get request on rss feeds.</param>
        public RssFeedAnalyzer(Feeds feeds, Client httpClient)
        {
            HttpClient = httpClient;
            RssWebhookEntities = feeds.FeedList;
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
                    foreach (var item in items)
                    {
                        if (ParseRssFeed(item, out var feedInfoItem) == false)
                        {
                            continue;
                        }

                        entity.StoredFeeds.Add(feedInfoItem);
                    }

                    continue;
                }

                foreach (var item in items)
                {
                    if (store.Any(x => x.Id == item.Element("link").Value)
                        || ParseRssFeed(item, out var feedInfoItem) == false)
                    {
                        continue;
                    }

                    msgsToSend.Add(new RawItem(item, entity));

                    store.Add(feedInfoItem);
                    if (store.Count > 150)
                    {
                        store.RemoveAt(0);
                    }
                }
            }

            return msgsToSend;
        }

        private bool ParseRssFeed(XElement rssFeedElement, out FeedInfoItem feedInfoItem)
        {
            feedInfoItem = null;
            if (DateTime.TryParse(rssFeedElement.Element("pubDate").Value, out var date) == false)
            {
                return false;
            }

            feedInfoItem = new FeedInfoItem(
                rssFeedElement.Element("link").Value,
                rssFeedElement.Element("title").Value,
                date);

            return true;
        }
    }
}
