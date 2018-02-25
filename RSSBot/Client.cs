using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using RSSBot.DataModel;
using RSSBot.Logging;

namespace RSSBot
{
    public class Client
    {
        private readonly ILogger _logger;

        public Client(ILogger logger)
        {
            _logger = logger;
        }

        public async Task TrySendMessageToDiscord(IList<ParsedItem> messages)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    foreach (var msg in messages)
                    {
                        var resp = await client.PostAsync($"{msg.Url}?wait=true", new StringContent(msg.ParsedMessage.ToString(), Encoding.UTF8, "application/json"));
                        if (resp.StatusCode != HttpStatusCode.OK)
                        {
                            throw new Exception($"{await resp.Content.ReadAsStringAsync()}{Environment.NewLine}{resp.StatusCode}");
                        }

                        _logger.LogInfo("Message Send. Everything OK!");
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task<IList<XElement>> TryGetFeeds(string url)
        {
            var returnValue = new List<XElement>();
            try
            {
                using (var client = new HttpClient())
                {
                    using (var stream = await client.GetStreamAsync(url))
                    {
                        returnValue = XDocument.Load(stream).Descendants("item").ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} {url}");
            }

            returnValue.Reverse();
            return returnValue;
        }
    }
}
