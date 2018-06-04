namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLastDownloadDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CardApplications", "LastDownloadDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CardApplications", "LastDownloadDate");
        }
    }
}
