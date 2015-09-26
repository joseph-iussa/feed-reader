namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CascadeFeedItemDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FeedItems", "Feed_ID", "dbo.Feeds");
            DropIndex("dbo.FeedItems", new[] { "Feed_ID" });
            AlterColumn("dbo.FeedItems", "Feed_ID", c => c.Int(nullable: false));
            CreateIndex("dbo.FeedItems", "Feed_ID");
            AddForeignKey("dbo.FeedItems", "Feed_ID", "dbo.Feeds", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FeedItems", "Feed_ID", "dbo.Feeds");
            DropIndex("dbo.FeedItems", new[] { "Feed_ID" });
            AlterColumn("dbo.FeedItems", "Feed_ID", c => c.Int());
            CreateIndex("dbo.FeedItems", "Feed_ID");
            AddForeignKey("dbo.FeedItems", "Feed_ID", "dbo.Feeds", "ID");
        }
    }
}
