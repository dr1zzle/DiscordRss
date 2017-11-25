﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RSSBot
{
    class Client
    {
        public async Task SendMessageToDiscord(string webhook, IList<DiscordWebhookMessage> messages)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    foreach (var msg in messages)
                    {
                        var responseMessage = await client.PostAsync(webhook, new StringContent(msg.ToString(), Encoding.UTF8, "application/json"));
                        WriteLogFile("Message Send. Everything OK!");
                        Thread.Sleep(2000);
                    }
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                WriteLogFile(ex.ToString());
            }
        }

        private void WriteLogFile(string msg)
        {
            using (var fileStream = new StreamWriter(File.Open("SendMsgLog.Txt", FileMode.Append)))
            {
                fileStream.WriteLine(msg + DateTime.Now.ToString());
            }
        }
    }
}