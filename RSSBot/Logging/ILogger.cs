namespace RSSBot.Logging
{
    public interface ILogger
    {
        void LogError(string errorMsg);
        void LogInfo(string infoMsg);
    }
}
