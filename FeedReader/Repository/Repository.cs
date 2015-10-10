using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedReader.Model;
using FeedReader.ViewModel;

namespace FeedReader.Repository
{
    class DataRepository : IDisposable
    {
        private DB db;

        public DataRepository(DB db)
        {
            this.db = db;
            this.db.Database.Log = (string s) => System.Diagnostics.Trace.TraceInformation(s);
        }

        public event EventHandler<FeedAddedEventArgs> FeedAdded;

        public event EventHandler<FeedModifiedEventArgs> FeedModified;

        public event EventHandler<FeedDeletedEventArgs> FeedDeleted;

        public event EventHandler<FeedItemsAddedEventArgs> FeedItemsAdded;

        public event EventHandler<FeedItemsDeletedEventArgs> FeedItemsDeleted;

        public IEnumerable<FeedViewModel> AllFeeds()
        {
            return db.Feeds.ToList().Select(feed => new FeedViewModel(this, feed));
        }

        public IEnumerable<FeedItemViewModel> AllFeedItems()
        {
            return db.FeedItems.Include("Feed").ToList()
                     .Select(feedItem => new FeedItemViewModel(feedItem));
        }

        public void AddFeed(Feed feed)
        {
            db.Feeds.Add(feed);
            db.SaveChanges();

            if (FeedAdded != null)
            {
                FeedAdded(this, new FeedAddedEventArgs(feed));
            }
        }

        public void ModifyFeed(Feed feed)
        {
            db.Entry(feed).State = EntityState.Modified;
            db.SaveChanges();

            if (FeedModified != null)
            {
                FeedModified(this, new FeedModifiedEventArgs(feed));
            }
        }

        public void DeleteFeed(Feed feed)
        {
            // Feed deletion handlers need relationships to FeedItem entities to be intact, so fire
            // event before db deletion.
            if (FeedDeleted != null)
            {
                FeedDeleted(this, new FeedDeletedEventArgs(feed));
            }

            db.Feeds.Remove(feed);
            db.SaveChanges();
        }

        public bool FeedExists(Feed feed)
        {
            return db.Feeds.Any(f => f.ID == feed.ID);
        }

        private int feedItemCount()
        {
            return db.FeedItems.Count();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}