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

        public async Task<List<DiscordWebhookMessage>> GetRssMessagesToSend()
        {
            var msgsToSend = await GetNewRssFeeds();
            var returnList = new List<DiscordWebhookMessage>();
            foreach (var msg in msgsToSend)
            {
                var setupMsg = RssWebhookEntities.First(x => x.Url == msg.Key).WebhookMessageTemplate;
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
                returnList.Add(setupMsg);
            }
            return returnList;
        }

        private async Task<Dictionary<string, XElement>> GetNewRssFeeds()
        {
            var msgsToSend = new Dictionary<string, XElement>();
            try
            {
                using (var rssClient = new HttpClient())
                {
                    foreach (var entity in RssWebhookEntities)
                    {
                        var items = XDocument.Load(await rssClient.GetStreamAsync(entity.Url)).Descendants("item").ToList();
                        if (entity.LastFeeds.Count != 0)
                        {
                            msgsToSend.Clear();
                            foreach (var item in items)
                                if (!entity.LastFeeds.Contains(item.Element("link").Value))
                                    msgsToSend.Add(entity.Url, item);
                        }
                        entity.LastFeeds = items.Select(x => x.Element("link").Value).ToList();
                    }
                    rssClient.Dispose();
                }
            }
            catch (Exception ex)
            {
                Program.WriteToLogFile("GetRssLog.Txt", ex + DateTime.Now.ToString());
            }
            return msgsToSend;
        }
    }
}
