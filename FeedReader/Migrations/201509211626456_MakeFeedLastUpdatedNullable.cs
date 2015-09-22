namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeFeedLastUpdatedNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Feeds", "LastUpdated", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Feeds", "LastUpdated", c => c.DateTime(nullable: false));
        }
    }
}
