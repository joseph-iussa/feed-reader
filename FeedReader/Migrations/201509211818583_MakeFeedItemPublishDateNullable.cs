namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeFeedItemPublishDateNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FeedItems", "PublishDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FeedItems", "PublishDate", c => c.DateTime(nullable: false));
        }
    }
}
