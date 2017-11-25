using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RSSBot
{
    class Program
    {
        private static Client Client;
        private static List<string> Webhooks;
        private static RssFeedAnalyzer RssAnalyzer;
        private static List<DiscordWebhookMessage> Messages;

        static void Main(string[] args)
        {
            RssAnalyzer = new RssFeedAnalyzer(JObject.Parse(File.ReadAllText("urls.txt")));
            Client = new Client();
            Webhooks = XDocument.Load("webhooks.txt").Element("webhooks").Elements("webhook").Select(x => x.Value).ToList();
            Console.WriteLine("RSSBot .NET CORE v1.4 by dr1zzle started ...");
            Task.Run(() => Run());
            Thread.Sleep(-1);
        }

        private static async Task Run()
        {
            while (true)
            {
                Messages = await RssAnalyzer.CheckIfNewFeedIsAvailable();
                Messages.Reverse();
                if (Messages.Count != 0)
                    foreach(var webhook in Webhooks)
                        await Client.SendMessageToDiscord(webhook, Messages);
                Thread.Sleep(300000);
            }
        }
    }
}