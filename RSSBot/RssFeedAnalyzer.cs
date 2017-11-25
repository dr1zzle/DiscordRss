using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace RSSBot
{
    class RssFeedAnalyzer
    {
        private JObject RssFeeds;
        private List<string> Urls;
        private List<XElement> msgsToSend;
        private List<DiscordWebhookMessage> returnList;
        private Dictionary<string, List<string>> LastRss;


        public RssFeedAnalyzer(JObject rssFeeds)
        {
            Urls = rssFeeds.Properties().Select(x => x.Name.ToString()).ToList();
            RssFeeds = rssFeeds;
            returnList = new List<DiscordWebhookMessage>();
            msgsToSend = new List<XElement>();
            LastRss = new Dictionary<string, List<string>>();
            foreach (var url in Urls)
                LastRss.Add(url, null);
        }

        public async Task<List<DiscordWebhookMessage>> CheckIfNewFeedIsAvailable()
        {
            returnList.Clear();
            try
            {
                using (var rssClient = new HttpClient())
                {
                    foreach (var url in Urls)
                    {
                        XNamespace xNamespace = "http://search.yahoo.com/mrss/";
                        var items = XDocument.Load(await rssClient.GetStreamAsync(url)).Descendants("item").ToList();
                        if (LastRss[url] != null)
                        {
                            msgsToSend.Clear();
                            foreach (var item in items)
                                if (!LastRss[url].Contains(item.Element("link").Value))
                                    msgsToSend.Add(item);

                            foreach (var msg in msgsToSend)
                            {
                                var setupMsg = RssFeeds.Property(url).Value.ToObject<DiscordWebhookMessage>();
                                setupMsg.embeds.First().url = msg.Element("link").Value;
                                setupMsg.embeds.First().title = msg.Element("title").Value;
                                var media = msg.Element(xNamespace + "content");
                                if (media != null)
                                {
                                    var thumbnail = media.Elements(xNamespace + "thumbnail").FirstOrDefault();
                                    if (thumbnail != null)
                                        setupMsg.embeds.First().thumbnail = new DiscordEmbedThumbnail
                                        {
                                            height = 50,
                                            width = 50,
                                            url = thumbnail.Attribute("url").Value
                                        };
                                }
                                setupMsg.embeds.First().description = WebUtility.HtmlDecode(Regex.Replace(msg.Element("description").Value.Replace("\n", string.Empty).Replace("\t", string.Empty), "<.*?>", string.Empty));
                                returnList.Add(setupMsg);
                            }
                        }
                        LastRss[url] = items.Select(x => x.Element("link").Value).ToList();
                    }
                    rssClient.Dispose();
                }
            }
            catch (Exception ex)
            {
                using (var fileStream = new StreamWriter(File.Open("GetRssLog.Txt", FileMode.Append)))
                {
                    fileStream.WriteLine(ex + DateTime.Now.ToString());
                }
            }
            return returnList;
        }

        public
    }
}
