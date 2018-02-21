namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newmodels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CardApplications", "isProcessed", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CardApplications", "isProcessed");
        }
    }
}
