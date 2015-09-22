namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHtmlDisplayFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeedItems", "HtmlHeader", c => c.String());
            AddColumn("dbo.FeedItems", "HtmlMainContent", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FeedItems", "HtmlMainContent");
            DropColumn("dbo.FeedItems", "HtmlHeader");
        }
    }
}
