using System;
using System.Threading.Tasks;

using RSSBot.Configuration;
using RSSBot.Configuration.ConfigModels;
using RSSBot.Logging;

namespace RSSBot
{
    public class Program
    {
        private static readonly ILogger _logger = new Logger(FileLocations.Log);

        private static Client Client;
        private static RssFeedAnalyzer RssAnalyzer;

        public static void Main(string[] args)
        {
            var configParser = new ConfigParser(_logger, FileLocations.Feeds, FileLocations.Config);
            var config = configParser.GetConfig();
            var rssWebhookEntities = configParser.GetFeeds();

            Client = new Client(_logger);
            RssAnalyzer = new RssFeedAnalyzer(rssWebhookEntities, Client);

            Task.Run(async () => await Run(config));
            Console.ReadKey();
        }

        private static async Task Run(Config config)
        {
            try
            {
                Console.WriteLine("Application started ...");
                while (true)
                {
                    var messages = await RssAnalyzer.GetRssMessagesToSend();
                    if (messages.Count != 0)
                    {
                        await Client.TrySendMessageToDiscord(messages);
                    }

                    await Task.Delay(TimeSpan.FromMinutes(config.CycleTime));
                }
            } 
            catch (Exception ex)
            {
                _logger.LogError($"STARTUP {ex.Message}");
            }
        }
    }
}