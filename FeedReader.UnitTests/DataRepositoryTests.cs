using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedReader.Model;
using FeedReader.Repository;
using Moq;
using NUnit.Framework;

namespace FeedReader.UnitTests
{
    [TestFixture]
    class DataRepositoryTests
    {
        private DataRepository repoUnderTest;

        private Mock<IDB> mockContext;
        private Mock<DbSet<Feed>> mockFeedSet;
        private Mock<DbSet<FeedItem>> mockFeedItemSet;

        private Feed singleFeed;

        [SetUp]
        public void SetUp()
        {
            mockContext = new Mock<IDB>();
            mockFeedSet = new Mock<DbSet<Feed>>();
            mockFeedItemSet = new Mock<DbSet<FeedItem>>();

            mockContext.Setup(m => m.Feeds).Returns(mockFeedSet.Object);
            mockContext.Setup(m => m.FeedItems).Returns(mockFeedItemSet.Object);

            // Do nothing for call to Include.
            mockFeedItemSet.Setup(m => m.Include(It.IsAny<string>())).Returns(mockFeedItemSet.Object);

            singleFeed = new Feed { ID = 42, Url = "http://www.something.com", Title = "Something" };

            repoUnderTest = new DataRepository(mockContext.Object);
        }

        [Test]
        public void Constructor_NullInput_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new DataRepository(null));
        }

        [TestCase("AddFeed")]
        [TestCase("ModifyFeed")]
        [TestCase("DeleteFeed")]
        [TestCase("FeedExists")]
        public void Various_NullInput_Throws(string methodName)
        {
            var mi = typeof(DataRepository).GetMethod(methodName);
            try
            {
                mi.Invoke(repoUnderTest, new object[] { null });
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Assert.IsInstanceOf<ArgumentNullException>(ex.InnerException);
                return;
            }

            Assert.Fail($"{methodName} did not throw on null input.");
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
        public void ModifyFeed_RaisesFeedModifiedEvent()
        {
            mockContext.Setup(m => m.GetEntityState(It.IsAny<Feed>())).Returns(EntityState.Modified);

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
    }
}