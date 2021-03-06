﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedReader.Model;

namespace FeedReader.Repository
{
    class DataRepository : IDisposable
    {
        private IDB db;

        public DataRepository(IDB db)
        {
            this.db = db.ThrowIfNull();
        }

        public event EventHandler<FeedAddedEventArgs> FeedAdded;

        public event EventHandler<FeedModifiedEventArgs> FeedModified;

        public event EventHandler<FeedDeletedEventArgs> FeedDeleted;

        public event EventHandler<FeedItemsAddedEventArgs> FeedItemsAdded;

        public event EventHandler<FeedItemsDeletedEventArgs> FeedItemsDeleted;

        public IEnumerable<Feed> AllFeeds()
        {
            return db.Feeds.ToList();
        }

        public IEnumerable<FeedItem> AllFeedItems()
        {
            return db.FeedItems.Include("Feed").ToList();
        }

        public void AddFeed(Feed feed)
        {
            feed.ThrowIfNull();

            db.Feeds.Add(feed);
            db.SaveChanges();

            if (FeedAdded != null)
            {
                FeedAdded(this, new FeedAddedEventArgs(feed));
            }
        }

        public void ModifyFeed(Feed feed)
        {
            feed.ThrowIfNull();
            if (db.GetEntityState(feed) != EntityState.Modified) return;

            db.SaveChanges();

            if (FeedModified != null)
            {
                FeedModified(this, new FeedModifiedEventArgs(feed));
            }
        }

        public void DeleteFeed(Feed feed)
        {
            feed.ThrowIfNull();

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
            feed.ThrowIfNull();

            return db.Feeds.Any(f => f.ID == feed.ID);
        }

        private int feedItemCount()
        {
            return db.FeedItems.Count();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
            }
        }
    }
}