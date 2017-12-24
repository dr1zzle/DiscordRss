using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using RSSBot.DataModel;

namespace RSSBot
{
    internal class Client
    {
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
                            throw new Exception($"{await resp.Content.ReadAsStringAsync()}{Environment.NewLine}{resp.StatusCode}");
                        Program.WriteToLogFile(LoggingLocations.SendMessage, "Message Send. Everything OK!");
                        await Task.Delay(TimeSpan.FromSeconds(2));
                    }
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                Program.WriteToLogFile(LoggingLocations.SendMessage, ex.Message);
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
                        stream.Dispose();
                    }
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                Program.WriteToLogFile(LoggingLocations.GetFeed, $"{ex.Message} {DateTime.Now} {url}");
            }
            returnValue.Reverse();
            return returnValue;
        }
    }
}
