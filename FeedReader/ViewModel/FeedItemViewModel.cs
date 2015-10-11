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
        private readonly FeedItem feedItem;
        private readonly StaticFeedViewModel feedViewModel;

        public FeedItemViewModel(FeedItem feedItem)
        {
            this.feedItem = feedItem;
            feedViewModel = new StaticFeedViewModel(this.feedItem.Feed);
        }

        public StaticFeedViewModel Feed { get { return feedViewModel; } }
        public string Title { get { return feedItem.Title; } }
        public DateTime? PublishDate { get { return feedItem.PublishDate; } }
        public string HtmlHeader { get { return feedItem.HtmlHeader; } }
        public string HtmlMainContent { get { return feedItem.HtmlMainContent; } }
    }
}