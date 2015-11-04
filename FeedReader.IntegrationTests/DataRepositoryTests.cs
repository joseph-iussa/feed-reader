using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedReader.Model;
using FeedReader.Repository;
using NUnit.Framework;

namespace FeedReader.IntegrationTests
{
    [TestFixture]
    class DataRepositoryTests
    {
        private DB context;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            if (File.Exists("testdb.sdf")) File.Delete("testdb.sdf");

            context = new DB();
            context.Database.Create();
        }

        [SetUp]
        public void SetUp()
        {
            context.Database.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            context.Database.CurrentTransaction.Rollback();
        }

        [Test]
        public void AllFeeds_ReturnsAllFeeds()
        {
            context.Feeds.Add(new Feed { Title = "test feed 1", Url = "url" });
            context.Feeds.Add(new Feed { Title = "test feed 2", Url = "url" });
            context.Feeds.Add(new Feed { Title = "test feed 3", Url = "url" });
            context.SaveChanges();

            DataRepository underTest = new DataRepository(context);

            List<Feed> returnedFeeds = new List<Feed>(underTest.AllFeeds());

            Assert.AreEqual(context.Feeds.Count(), returnedFeeds.Count(),
                            "Returned AllFeeds collection is the wrong length.");

            Assert.Contains(context.Feeds.Single(f => f.Title == "test feed 1"), returnedFeeds);
            Assert.Contains(context.Feeds.Single(f => f.Title == "test feed 2"), returnedFeeds);
            Assert.Contains(context.Feeds.Single(f => f.Title == "test feed 3"), returnedFeeds);
        }

        [Test]
        public void AllFeedItems_ReturnsAllFeedItems()
        {
            Feed feed = new Feed { Title = "test feed", Url = "url" };
            context.Feeds.Add(feed);
            context.FeedItems.Add(new FeedItem { Feed = feed, Title = "feed item 1" });
            context.FeedItems.Add(new FeedItem { Feed = feed, Title = "feed item 2" });
            context.FeedItems.Add(new FeedItem { Feed = feed, Title = "feed item 3" });
            context.SaveChanges();

            DataRepository underTest = new DataRepository(context);

            List<FeedItem> returnedFeedItems = new List<FeedItem>(underTest.AllFeedItems());

            Assert.AreEqual(context.FeedItems.Count(), returnedFeedItems.Count(),
                            "Returned AllFeedItems collection is the wrong length.");

            Assert.Contains(context.FeedItems.Single(fi => fi.Title == "feed item 1"), returnedFeedItems);
            Assert.Contains(context.FeedItems.Single(fi => fi.Title == "feed item 2"), returnedFeedItems);
            Assert.Contains(context.FeedItems.Single(fi => fi.Title == "feed item 3"), returnedFeedItems);
        }

        [Test]
        public void AddFeed_WithValidFeed_AddsAndSavesFeed()
        {
            Feed newFeed = new Feed { Title = "new feed", Url = "url" };
            DataRepository underTest = new DataRepository(context);

            underTest.AddFeed(newFeed);

            Assert.Contains(newFeed, context.Feeds.ToList());
        }

        [Test]
        public void ModifyFeed_WithValidFeed_ModifiesAndSavesChangesToFeed()
        {
            context.Feeds.Add(new Feed { Title = "test feed 1", Url = "url" });
            context.SaveChanges();

            DataRepository underTest = new DataRepository(context);

            Feed feedToModify = context.Feeds.First();
            feedToModify.Title += " MODIFIED";
            underTest.ModifyFeed(feedToModify);

            var feedTitleProperty = context.Entry(feedToModify).Property(f => f.Title);
            Assert.AreEqual(feedTitleProperty.CurrentValue, feedTitleProperty.OriginalValue);
        }

        [Test]
        public void DeleteFeed_WithValidFeed_DeletesFeed()
        {
            context.Feeds.Add(new Feed { Title = "test feed 1", Url = "url" });
            context.SaveChanges();

            DataRepository underTest = new DataRepository(context);

            Feed feedToDelete = context.Feeds.First();
            underTest.DeleteFeed(feedToDelete);

            Assert.IsFalse(context.Feeds.Any(f => f.ID == feedToDelete.ID));
        }

        [Test]
        public void FeedExists_WhenFeedExists_ReturnsTrue()
        {
            Feed existingFeed = new Feed { Title = "feed", Url = "url" };
            context.Feeds.Add(existingFeed);
            context.SaveChanges();

            DataRepository underTest = new DataRepository(context);

            Assert.IsTrue(underTest.FeedExists(existingFeed));
        }

        [Test]
        public void FeedExists_WhenFeedDoesNotExist_ReturnsFalse()
        {
            DataRepository underTest = new DataRepository(context);
            Feed nonExistantFeed = new Feed { Title = "feed", Url = "url" };

            Assert.IsFalse(underTest.FeedExists(nonExistantFeed));
        }
    }
}