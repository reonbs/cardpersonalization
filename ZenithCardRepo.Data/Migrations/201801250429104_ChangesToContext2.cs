namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangesToContext2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CardApplications", "ReferenceNo", c => c.String());
            AddColumn("dbo.CardApplications", "ImageLocation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CardApplications", "ImageLocation");
            DropColumn("dbo.CardApplications", "ReferenceNo");
        }
    }
}
