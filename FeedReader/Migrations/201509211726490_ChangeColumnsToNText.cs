namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeColumnsToNText : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FeedItems", "Summary", c => c.String());
            AlterColumn("dbo.FeedItems", "Content", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FeedItems", "Content", c => c.String(maxLength: 4000));
            AlterColumn("dbo.FeedItems", "Summary", c => c.String(maxLength: 4000));
        }
    }
}
