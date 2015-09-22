namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsReadToFeedItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeedItems", "IsRead", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FeedItems", "IsRead");
        }
    }
}
