using System;

namespace RSSBot.DataModel
{
    internal class FeedInfoItem
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FeedInfoItem"/>.
        /// </summary>
        public FeedInfoItem() { }

        /// <summary>
        /// Initializes a new instance of <see cref="FeedInfoItem"/>.
        /// </summary>
        /// <param name="id">GUID of the feed.</param>
        /// <param name="pubDate">Publishing date of the feed.</param>
        /// <param name="title">Title of the feed.</param>
        public FeedInfoItem(string id, string title, DateTime pubDate)
        {
            Id = id;
            Title = title;
            PubDate = pubDate;
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime PubDate { get; set; }
    }
}
