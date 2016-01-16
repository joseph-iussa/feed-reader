using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedReader.Model;

namespace FeedReader.ViewModel
{
    class StaticFeedViewModel : ViewModelBase,
                                IEquatable<StaticFeedViewModel>,
                                IEquatable<Feed>
    {
        protected readonly Feed feed;

        public StaticFeedViewModel(Feed feed)
        {
            this.feed = feed.ThrowIfNull();
        }

        public string Title { get { return feed.Title; } }
        public string Url { get { return feed.Url; } }

        public override bool Equals(object obj)
        {
            if (obj is StaticFeedViewModel)
            {
                return Equals((StaticFeedViewModel)obj);
            }
            else if (obj is Feed)
            {
                return Equals((Feed)obj);
            }

            return false;
        }

        public bool Equals(StaticFeedViewModel other)
        {
            return feed.ID == other.ThrowIfNull().feed.ID;
        }

        public bool Equals(Feed other)
        {
            return feed.ID == other.ThrowIfNull().ID;
        }

        public override int GetHashCode()
        {
            return feed.ID.GetHashCode();
        }

        public static bool operator ==(StaticFeedViewModel left, object right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(StaticFeedViewModel left, object right)
        {
            return !(left == right);
        }
    }
}