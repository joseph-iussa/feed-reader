using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedReader.Model;

namespace FeedReader.ViewModel
{
    class FeedItemViewModel : ViewModelBase
    {
        private FeedItem feedItem;

        public FeedItemViewModel(FeedItem feedItem)
        {
            this.feedItem = feedItem;
        }

        public int ID { get { return feedItem.ID; } }
        public Feed Feed { get { return feedItem.Feed; } }
        public string FeedTitle { get { return feedItem.Feed.Title; } }
        public string Title { get { return feedItem.Title; } }
        public DateTime? PublishDate { get { return feedItem.PublishDate; } }
        public string HtmlHeader { get { return feedItem.HtmlHeader; } }
        public string HtmlMainContent { get { return feedItem.HtmlMainContent; } }
    }
}