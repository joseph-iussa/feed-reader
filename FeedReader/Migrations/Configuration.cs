namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Model;

    internal sealed class Configuration : DbMigrationsConfiguration<FeedReader.Model.DB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "FeedReader.Model.DB";
        }

        protected override void Seed(FeedReader.Model.DB db)
        {
            db.Feeds.AddOrUpdate(
                new Feed { Title = "Video Games", Url = "whatever" },
                new Feed { Title = "Movies", Url = "whatever" },
                new Feed { Title = "Programming", Url = "whatever" }
            );
        }
    }
}