using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Newtonsoft.Json.Linq;

namespace RSSBot
{
    public class Program
    {
        private static Client Client;
        private static List<string> Webhooks;
        private static RssFeedAnalyzer RssAnalyzer;
        private static List<DiscordWebhookMessage> Messages;

        public static void Main(string[] args)
        {
            var rssWebhookEntities = GetRssWebhookEntities();
            Webhooks = GetWebhooks();

            Client = new Client();
            RssAnalyzer = new RssFeedAnalyzer(rssWebhookEntities);

            Task.Run(() => Run());
            Console.ReadKey();
        }

        public static async Task WriteToLogFile(string destinationFile, string msg)
        {
            using (var fileStream = new StreamWriter(File.Open(destinationFile, FileMode.Append)))
            {
                await fileStream.WriteLineAsync(msg + DateTime.Now.ToString());
            }
        }

        private static async Task Run()
        {
            Console.WriteLine("Application started ...");
            while (true)
            {
                Messages = await RssAnalyzer.GetRssMessagesToSend();
                Messages.Reverse();
                if (Messages.Count != 0)
                    foreach(var webhook in Webhooks)
                        await Client.SendMessageToDiscord(webhook, Messages);
                Thread.Sleep(300000);
            }
        }

        private static List<RssWebhookEntity> GetRssWebhookEntities()
        {
            var returnValue = new List<RssWebhookEntity>();
            var rawJObject = JObject.Parse(File.ReadAllText("urls.txt"));
            foreach (var property in rawJObject.Properties())
                returnValue.Add(new RssWebhookEntity {
                    Url = property.Name,
                    WebhookMessageTemplate = property.Value.ToObject<DiscordWebhookMessage>()
                });
            return returnValue;
        }

        private static List<string> GetWebhooks()
        {
            return XDocument.Load("webhooks.txt").Descendants("webhook").Select(x => x.Value).ToList();
        }
    }
}