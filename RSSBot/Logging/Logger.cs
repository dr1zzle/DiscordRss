using System;
using System.IO;

namespace RSSBot.Logging
{
    public class Logger : ILogger
    {
        private readonly string _logFilePath;

        public Logger(string logFilePath)
        {
            _logFilePath = logFilePath;
        } 

        public void LogError(string errorMsg)
        {
            using (var fileStream = new StreamWriter(File.Open(_logFilePath, FileMode.Append)))
            {
                fileStream.WriteLine($"{DateTime.Now} ERROR | {errorMsg}");
            }
        }

        public void LogInfo(string infoMsg)
        {
            using (var fileStream = new StreamWriter(File.Open(_logFilePath, FileMode.Append)))
            {
                fileStream.WriteLine($"{DateTime.Now} INFO | {infoMsg}");
            }
        }
    }
}
