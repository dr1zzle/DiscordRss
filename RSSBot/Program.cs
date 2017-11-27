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
        private static RssFeedAnalyzer RssAnalyzer;

        public static void Main(string[] args)
        {
            var rssWebhookEntities = GetRssWebhookEntities();

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
                var messages = await RssAnalyzer.GetRssMessagesToSend();
                messages.Reverse();
                if (messages.Count != 0)
                    await Client.SendMessageToDiscord(messages);
                Thread.Sleep(120000);
            }
        }

        private static List<RssWebhookEntity> GetRssWebhookEntities()
        {
            var returnValue = new List<RssWebhookEntity>();
            var rawJObject = JObject.Parse(File.ReadAllText("urls.txt"));
            foreach (var property in rawJObject.Properties())
                returnValue.Add(new RssWebhookEntity {
                    Url = property.Name,
                    Webhook = property.Value["webhook"].ToString(),
                    WebhookMessageTemplate = property.Value["template"].ToObject<DiscordWebhookMessage>()
                });
            return returnValue;
        }
    }
}