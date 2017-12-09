using System;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RSSBot
{
    internal class Client
    {
        public async Task SendMessageToDiscord(IList<KeyValuePair<string, DiscordWebhookMessage>> messages)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    foreach (var msg in messages)
                    {
                        var responseMessage = await client.PostAsync(msg.Key, new StringContent(msg.Value.ToString(), Encoding.UTF8, "application/json"));
                        await Program.WriteToLogFile("SendMsgLog.Txt", "Message Send. Everything OK!");
                        Thread.Sleep(2000);
                    }
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                await Program.WriteToLogFile("SendMsgLog.Txt", ex.ToString());
            }
        }
    }
}
