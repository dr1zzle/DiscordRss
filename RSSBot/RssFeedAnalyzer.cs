using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace RSSBot
{
    internal class RssFeedAnalyzer
    {
        private XNamespace XNamespace;
        private List<RssWebhookEntity> RssWebhookEntities;

        public RssFeedAnalyzer(List<RssWebhookEntity> rssWebhookentities)
        {
            XNamespace = "http://search.yahoo.com/mrss/";
            RssWebhookEntities = rssWebhookentities;
        }

        public async Task<List<KeyValuePair<string, DiscordWebhookMessage>>> GetRssMessagesToSend()
        {
            var msgsToSend = await GetNewRssFeeds();
            var returnList = new List<KeyValuePair<string, DiscordWebhookMessage>>();
            foreach (var msg in msgsToSend)
            {
                var setupMsg = RssWebhookEntities.First(x => x.Url == msg.Key).WebhookMessageTemplate.DeepCopy();
                setupMsg.embeds.First().url = msg.Value.Element("link").Value;
                setupMsg.embeds.First().title = msg.Value.Element("title").Value;
                var media = msg.Value.Element(XNamespace + "content");
                if (media != null)
                {
                    var thumbnail = media.Elements(XNamespace + "thumbnail").FirstOrDefault();
                    if (thumbnail != null)
                        setupMsg.embeds.First().thumbnail = new DiscordEmbedThumbnail
                        {
                            height = 50,
                            width = 50,
                            url = thumbnail.Attribute("url").Value
                        };
                }
                setupMsg.embeds.First().description = WebUtility.HtmlDecode(Regex.Replace(msg.Value.Element("description")
                    .Value.Replace("\n", string.Empty)
                    .Replace("\t", string.Empty), "<.*?>", string.Empty));
                returnList.Add(new KeyValuePair<string, DiscordWebhookMessage>(RssWebhookEntities.First(x => x.Url == msg.Key).Webhook, setupMsg));
            }
            return returnList;
        }

        private async Task<List<KeyValuePair<string, XElement>>> GetNewRssFeeds()
        {
            var msgsToSend = new List<KeyValuePair<string, XElement>>();
            try
            {
                using (var rssClient = new HttpClient())
                {
                    foreach (var entity in RssWebhookEntities)
                    {
                        var items = XDocument.Load(await rssClient.GetStreamAsync(entity.Url)).Descendants("item").ToList();
                        if (entity.LastFeeds.Count != 0)
                        {
                            foreach (var item in items)
                                if (!entity.LastFeeds.Contains(item.Element("link").Value))
                                {
                                    msgsToSend.Add(new KeyValuePair<string, XElement>(entity.Url, item));
                                    entity.LastFeeds.Add(item.Element("link").Value);
                                    if (entity.LastFeeds.Count > 55)
                                        entity.LastFeeds.Remove(entity.LastFeeds.First());
                                }
                        }
                        else
                        {
                            entity.LastFeeds = items.Select(x => x.Element("link").Value).ToList();
                        }
                    }
                    rssClient.Dispose();
                }
            }
            catch (Exception ex)
            {
                await Program.WriteToLogFile("GetRssLog.Txt", ex + DateTime.Now.ToString());
            }
            return msgsToSend;
        }
    }
}
