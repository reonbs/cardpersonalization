namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModel221 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CardApplications", "RequestingBranchCode", c => c.String(nullable: false));
            DropColumn("dbo.CardApplications", "RequestingBanchCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CardApplications", "RequestingBanchCode", c => c.String(nullable: false));
            DropColumn("dbo.CardApplications", "RequestingBranchCode");
        }
    }
}
