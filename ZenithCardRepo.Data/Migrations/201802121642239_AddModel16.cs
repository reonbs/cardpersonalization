namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModel16 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CardApplications", "ProcessedBatchNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CardApplications", "ProcessedBatchNo");
        }
    }
}
