namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFeedItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FeedItems",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 4000),
                        Url = c.String(maxLength: 4000),
                        Summary = c.String(maxLength: 4000),
                        Content = c.String(maxLength: 4000),
                        PublishDate = c.DateTime(nullable: false),
                        Feed_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Feeds", t => t.Feed_ID)
                .Index(t => t.Feed_ID);
            
            AddColumn("dbo.Feeds", "LastUpdated", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FeedItems", "Feed_ID", "dbo.Feeds");
            DropIndex("dbo.FeedItems", new[] { "Feed_ID" });
            DropColumn("dbo.Feeds", "LastUpdated");
            DropTable("dbo.FeedItems");
        }
    }
}
