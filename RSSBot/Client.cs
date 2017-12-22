using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading;
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
                        var responseMessage = await client.PostAsync(msg.Url, new StringContent(msg.ParsedMessage.ToString(), Encoding.UTF8, "application/json"));
                        Program.WriteToLogFile(LoggingLocations.SendMessage, "Message Send. Everything OK!");
                        Thread.Sleep(2000);
                    }
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                Program.WriteToLogFile(LoggingLocations.SendMessage, ex.ToString());
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
                Program.WriteToLogFile(LoggingLocations.GetFeed, ex + " " + DateTime.Now.ToString() + " " + url);
            }
            returnValue.Reverse();
            return returnValue;
        }
    }
}
