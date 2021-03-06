﻿using Newtonsoft.Json;

namespace RSSBot.DiscordWebhookModels
{
    public class DiscordWebhookObject
    {
        public override string ToString() => JsonConvert.SerializeObject(this);
        public virtual T Copy<T>() => (T)MemberwiseClone();
    }
}
