using Newtonsoft.Json;

namespace RSSBot
{
    internal class DiscordWebhookObject
    {
        public override string ToString() => JsonConvert.SerializeObject(this);
        public T Copy<T>() => (T)MemberwiseClone();
    }
}
