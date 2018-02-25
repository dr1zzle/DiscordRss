using System;

namespace RSSBot.DataModel
{
    public class FeedInfoItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime PubDate { get; set; }

        public FeedInfoItem(string id, string title, DateTime pubDate)
        {
            Id = id;
            Title = title;
            PubDate = pubDate;
        }
    }
}
