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
            var urlTemplateDictionary = GetUrlTemplateDictionary();
            Webhooks = GetWebhooks();

            Client = new Client();
            RssAnalyzer = new RssFeedAnalyzer(urlTemplateDictionary);

            Task.Run(() => Run());
            Console.ReadKey();
        }

        public static void WriteToLogFile(string destinationFile, string msg)
        {
            using (var fileStream = new StreamWriter(File.Open(destinationFile, FileMode.Append)))
            {
                fileStream.WriteLine(msg + DateTime.Now.ToString());
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

        private static Dictionary<string, DiscordWebhookMessage> GetUrlTemplateDictionary()
        {
            var returnValue = new Dictionary<string, DiscordWebhookMessage>();
            var rawJObject = JObject.Parse(File.ReadAllText("urls.txt"));
            foreach (var property in rawJObject.Properties())
                returnValue.Add(property.Name, property.Value.ToObject<DiscordWebhookMessage>());
            return returnValue;
        }

        private static List<string> GetWebhooks()
        {
            return XDocument.Load("webhooks.txt").Descendants("webhook").Select(x => x.Value).ToList();
        }
    }
}