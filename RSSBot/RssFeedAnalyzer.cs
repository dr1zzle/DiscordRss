﻿using System;
using System.Net;
using System.Linq;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RSSBot
{
    internal class RssFeedAnalyzer
    {
        private XNamespace XNamespace;
        private IList<RssWebhookEntity> RssWebhookEntities;

        public RssFeedAnalyzer(IList<RssWebhookEntity> rssWebhookentities)
        {
            XNamespace = "http://search.yahoo.com/mrss/";
            RssWebhookEntities = rssWebhookentities;
        }

        public async Task<IList<KeyValuePair<string, DiscordWebhookMessage>>> GetRssMessagesToSend()
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

        private async Task<IList<KeyValuePair<string, XElement>>> GetNewRssFeeds()
        {
            var msgsToSend = new List<KeyValuePair<string, XElement>>();
            using (var rssClient = new HttpClient())
            {
                foreach (var entity in RssWebhookEntities)
                {
                    var items = await TryGetFeeds(entity.Url, rssClient);
                    if (entity.LastFeeds.Count != 0)
                    {
                        foreach (var item in items)
                        {
                            var linkValue = item.Element("link").Value;
                            if (!entity.LastFeeds.Contains(linkValue) &&
                                !entity.SentFeeds.Contains(linkValue))
                            {
                                msgsToSend.Add(new KeyValuePair<string, XElement>(entity.Url, item));
                                entity.SentFeeds.Add(linkValue);
                                if (entity.SentFeeds.Count > 10)
                                    entity.SentFeeds.Remove(entity.SentFeeds.First());
                            }
                        }
                    }
                    entity.LastFeeds = items.Select(x => x.Element("link").Value).ToList();
                }
                rssClient.Dispose();
            }
            return msgsToSend;
        }

        private async Task<IList<XElement>> TryGetFeeds(string url, HttpClient httpClient)
        {
            var returnValue = new List<XElement>();
            try
            {
                using (var stream = await httpClient.GetStreamAsync(url))
                {
                    returnValue = XDocument.Load(stream).Descendants("item").ToList();
                    stream.Dispose();
                }
            }
            catch (Exception ex)
            {
                await Program.WriteToLogFile("GetRssLog.Txt", ex + " " + DateTime.Now.ToString() + " " + url);
            }
            return returnValue;
        }
    }
}
