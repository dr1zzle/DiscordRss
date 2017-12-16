using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using RSSBot.Configuration;
using RSSBot.Configuration.ConfigModels;

namespace RSSBot
{
    public class Program
    {
        private static Client Client;
        private static RssFeedAnalyzer RssAnalyzer;

        public static void Main(string[] args)
        {
            var configParser = new ConfigParser();
            var config = configParser.GetConfig();
            var rssWebhookEntities = configParser.GetFeeds();

            Client = new Client();
            RssAnalyzer = new RssFeedAnalyzer(rssWebhookEntities, Client);

            Task.Run(() => Run(config));
            Console.ReadKey();
        }

        public static async Task WriteToLogFile(string destinationFile, string msg)
        {
            using (var fileStream = new StreamWriter(File.Open(destinationFile, FileMode.Append)))
            {
                await fileStream.WriteLineAsync(msg + DateTime.Now.ToString());
            }
        }

        private static async Task Run(Config config)
        {
            Console.WriteLine("Application started ...");
            while (true)
            {
                var messages = await RssAnalyzer.GetRssMessagesToSend();
                if (messages.Count != 0)
                    await Client.TrySendMessageToDiscord(messages);
                Thread.Sleep(TimeSpan.FromMinutes(config.CycleTime));
            }
        }
    }
}