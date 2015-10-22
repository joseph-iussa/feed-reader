﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedReader.Model;
using FeedReader.Repository;
using Moq;
using NUnit.Framework;

namespace FeedReader.Tests
{
    [TestFixture]
    class DataRepositoryTests
    {
        private static FeedEqualityComparer feedEqualityComparer = new FeedEqualityComparer();

        private DataRepository repoUnderTest;

        private Mock<DB> mockContext;
        private Mock<DbSet<Feed>> mockFeedSet;
        private Mock<DbSet<FeedItem>> mockFeedItemSet;

        private List<Feed> allFeeds;
        private List<FeedItem> allFeedItems;
        private List<Feed> allFeedsPersisted;
        private List<FeedItem> allFeedItemsPersisted;

        private Feed singleFeed;

        [SetUp]
        public void SetUp()
        {
            mockContext = new Mock<DB>();
            mockFeedSet = new Mock<DbSet<Feed>>();
            mockFeedItemSet = new Mock<DbSet<FeedItem>>();

            mockContext.Setup(m => m.Feeds).Returns(mockFeedSet.Object);
            mockContext.Setup(m => m.FeedItems).Returns(mockFeedItemSet.Object);

            mockFeedSet.Setup(m => m.Remove(It.IsAny<Feed>())).Callback((Feed f) => allFeeds.Remove(f));

            allFeeds = new List<Feed>() {
                new Feed { ID = 1, Url = "http://www.something1.com", Title = "Something 1" },
                new Feed { ID = 2, Url = "http://www.something2.com", Title = "Something 2" },
                new Feed { ID = 3, Url = "http://www.something3.com", Title = "Something 3" }
            };

            var qAllFeeds = allFeeds.AsQueryable();
            mockFeedSet.As<IQueryable<Feed>>().Setup(m => m.Provider).Returns(qAllFeeds.Provider);
            mockFeedSet.As<IQueryable<Feed>>().Setup(m => m.Expression).Returns(qAllFeeds.Expression);
            mockFeedSet.As<IQueryable<Feed>>().Setup(m => m.ElementType).Returns(qAllFeeds.ElementType);
            mockFeedSet.As<IQueryable<Feed>>().Setup(m => m.GetEnumerator()).Returns(qAllFeeds.GetEnumerator());

            allFeedItems = new List<FeedItem> {
                new FeedItem {ID = 1, Feed = allFeeds[0], Title = "FeedItem 1" },
                new FeedItem {ID = 2, Feed = allFeeds[0], Title = "FeedItem 2" },
                new FeedItem {ID = 3, Feed = allFeeds[0], Title = "FeedItem 3" }
            };

            var qAllFeedItems = allFeedItems.AsQueryable();
            mockFeedItemSet.As<IQueryable<FeedItem>>().Setup(m => m.Provider).Returns(qAllFeedItems.Provider);
            mockFeedItemSet.As<IQueryable<FeedItem>>().Setup(m => m.Expression).Returns(qAllFeedItems.Expression);
            mockFeedItemSet.As<IQueryable<FeedItem>>().Setup(m => m.ElementType).Returns(qAllFeedItems.ElementType);
            mockFeedItemSet.As<IQueryable<FeedItem>>().Setup(m => m.GetEnumerator()).Returns(qAllFeedItems.GetEnumerator());

            // Do nothing for call to Include.
            // TODO: Do something for call to Include?
            mockFeedItemSet.Setup(m => m.Include(It.IsAny<string>())).Returns(mockFeedItemSet.Object);

            allFeedsPersisted = new List<Feed>(
                allFeeds.Select(f => new Feed { ID = f.ID, Url = f.Url, Title = f.Title }));
            // TODO: Properly assign persisted feed item feed references.
            allFeedItemsPersisted = new List<FeedItem>(
                allFeedItems.Select(fi => new FeedItem { ID = fi.ID, Feed = fi.Feed, Title = fi.Title }));

            mockContext.Setup(m => m.SaveChanges()).Callback(() =>
            {
                allFeedsPersisted.Clear();
                allFeedsPersisted.AddRange(allFeeds);
                allFeedItemsPersisted.Clear();
                allFeedItemsPersisted.AddRange(allFeedItems);
            });

            singleFeed = new Feed { ID = 42, Url = "http://www.something.com", Title = "Something" };

            repoUnderTest = new DataRepository(mockContext.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ThrowsOnNullInput()
        {
            new DataRepository(null);
        }

        [Test]
        public void AllFeeds_ReturnsAllFeeds()
        {
            IEnumerable<Feed> returnedFeeds = repoUnderTest.AllFeeds();

            Assert.AreEqual(allFeeds.Count, returnedFeeds.Count(),
                            "Returned AllFeeds collection is the wrong length.");

            for (int i = 0; i < allFeeds.Count; i++)
            {
                Assert.IsTrue(returnedFeeds.Contains(allFeeds[i]),
                              $"Feed {{ID: {allFeeds[i].ID}, Title: {allFeeds[i].Title}}} is " +
                              "missing from returned AllFeeds collection.");
            }
        }

        [Test]
        public void AllFeedItems_ReturnsAllFeedItems()
        {
            IEnumerable<FeedItem> returnedFeedItems = repoUnderTest.AllFeedItems();

            Assert.AreEqual(allFeedItems.Count, returnedFeedItems.Count(),
                            "Returned AllFeedItems collection is the wrong length.");

            for (int i = 0; i < allFeedItems.Count; i++)
            {
                Assert.IsTrue(returnedFeedItems.Contains(allFeedItems[i]),
                              $"FeedItem {{ID: {allFeedItems[i].ID}, Title: {allFeedItems[i].Title}}} is " +
                              "missing from returned AllFeedItems collection.");
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddFeed_ThrowsOnNullInput()
        {
            repoUnderTest.AddFeed(null);
        }

        // TODO: change this.
        [Test]
        public void AddFeed_AddsAndSavesFeedViaContext()
        {
            // Ensure calls are made in correct order.
            int callOrder = 0;
            string errMsg = "SaveChanges called but Add(feed) has not been called yet.";
            mockFeedSet.Setup(m => m.Add(singleFeed))
                       .Callback(() => Assert.That(callOrder++, Is.EqualTo(0), errMsg));
            mockContext.Setup(m => m.SaveChanges())
                       .Callback(() => Assert.That(callOrder, Is.EqualTo(1), errMsg));

            repoUnderTest.AddFeed(singleFeed);

            mockFeedSet.Verify(m => m.Add(singleFeed), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void AddFeed_RaisesFeedAddedEvent()
        {
            bool eventRaised = false;
            Feed eventArgFeed = null;

            repoUnderTest.FeedAdded += (object sender, FeedAddedEventArgs e) =>
            {
                eventRaised = true;
                eventArgFeed = e.AddedFeed;
            };

            repoUnderTest.AddFeed(singleFeed);

            Assert.IsTrue(eventRaised, "FeedAdded event not raised.");
            Assert.AreEqual(singleFeed, eventArgFeed, "Wrong Feed attached to FeedAdded event.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ModifyFeed_ThrowsOnNullInput()
        {
            repoUnderTest.ModifyFeed(null);
        }

        // TODO: Mock context Entry member.
        [Test]
        public void ModifyFeed_ModifiesAndSavesChangesToFeedViaContext()
        {
            int feedToModifyId = 1;
            Feed feedToModify = allFeeds.Single(f => f.ID == feedToModifyId);

            feedToModify.Title += " MODIFIED";
            repoUnderTest.ModifyFeed(feedToModify);

            Feed modifiedAndSavedFeed = allFeedsPersisted.Single(f => f.ID == feedToModifyId);
            Assert.AreEqual(feedToModify.Title, modifiedAndSavedFeed.Title,
                            "Feed changes not persisted to context.");
        }

        [Test]
        public void ModifyFeed_RaisesFeedModifiedEvent()
        {
            bool eventRaised = false;
            Feed eventArgFeed = null;

            repoUnderTest.FeedModified += (object sender, FeedModifiedEventArgs e) =>
            {
                eventRaised = true;
                eventArgFeed = e.ModifiedFeed;
            };

            repoUnderTest.ModifyFeed(singleFeed);

            Assert.IsTrue(eventRaised, "FeedModified event not raised.");
            Assert.AreEqual(singleFeed, eventArgFeed, "Wrong Feed attached to FeedModified event.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteFeed_ThrowsOnNullInput()
        {
            repoUnderTest.DeleteFeed(null);
        }

        [Test]
        public void DeleteFeed_DeletesFeedViaContext()
        {
            Feed feedToDelete = allFeeds.First();

            repoUnderTest.DeleteFeed(feedToDelete);

            Assert.IsFalse(allFeedsPersisted.Contains(feedToDelete, feedEqualityComparer),
                           "Feed not removed from context.");
        }

        [Test]
        public void DeleteFeed_RaisesFeedDeletedEvent()
        {
            bool eventRaised = false;
            Feed eventArgFeed = null;

            repoUnderTest.FeedDeleted += (object sender, FeedDeletedEventArgs e) =>
            {
                eventRaised = true;
                eventArgFeed = e.DeletedFeed;
            };

            repoUnderTest.DeleteFeed(singleFeed);

            Assert.IsTrue(eventRaised, "FeedDeleted event not raised.");
            Assert.AreEqual(singleFeed, eventArgFeed, "Wrong Feed attached to FeedDeleted event.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FeedExists_ThrowsOnNullInput()
        {
            repoUnderTest.FeedExists(null);
        }

        [Test]
        public void FeedExists_ReturnsTrueWhenFeedExists()
        {
            Assert.IsTrue(repoUnderTest.FeedExists(allFeeds.First()));
        }

        [Test]
        public void FeedExists_ReturnsFalseWhenFeedDoesNotExist()
        {
            Assert.IsFalse(repoUnderTest.FeedExists(singleFeed));
        }
    }

    class FeedEqualityComparer : IEqualityComparer<Feed>
    {
        public bool Equals(Feed x, Feed y)
        {
            return x.ID == y.ID;
        }

        public int GetHashCode(Feed obj)
        {
            return obj.ID.GetHashCode();
        }
    }
}