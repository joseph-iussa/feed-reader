using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedReader.Model;

namespace FeedReader
{
    class FeedAddedEventArgs : EventArgs
    {
        public FeedAddedEventArgs(Feed addedFeed)
        {
            AddedFeed = addedFeed.ThrowIfNull();
        }

        public Feed AddedFeed { get; private set; }
    }

    class FeedModifiedEventArgs : EventArgs
    {
        public FeedModifiedEventArgs(Feed modifiedFeed)
        {
            ModifiedFeed = modifiedFeed.ThrowIfNull();
        }

        public Feed ModifiedFeed { get; private set; }
    }

    class FeedDeletedEventArgs : EventArgs
    {
        public FeedDeletedEventArgs(Feed deletedFeed)
        {
            DeletedFeed = deletedFeed.ThrowIfNull();
        }

        public Feed DeletedFeed { get; private set; }
    }

    class FeedItemsAddedEventArgs : EventArgs
    {
        public FeedItemsAddedEventArgs(IEnumerable<FeedItem> addedFeedItems)
        {
            AddedFeedItems = addedFeedItems.ThrowIfNull();
        }

        public IEnumerable<FeedItem> AddedFeedItems { get; private set; }
    }

    class FeedItemsDeletedEventArgs : EventArgs
    {
        public FeedItemsDeletedEventArgs(IEnumerable<FeedItem> deletedFeedItems)
        {
            DeletedFeedItems = deletedFeedItems.ThrowIfNull();
        }

        public IEnumerable<FeedItem> DeletedFeedItems { get; private set; }
    }

    class DialogClosingEventArgs : EventArgs
    {
        public DialogClosingEventArgs(bool? dialogResult)
        {
            DialogResult = dialogResult;
        }

        public bool? DialogResult { get; private set; }
    }
}