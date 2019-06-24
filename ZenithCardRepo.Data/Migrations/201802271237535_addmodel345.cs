namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmodel345 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Approvals", "CardApplicationID", "dbo.CardApplications");
            DropIndex("dbo.Approvals", new[] { "CardApplicationID" });
            AddColumn("dbo.Approvals", "CardApplicationIDs", c => c.String());
            DropColumn("dbo.Approvals", "CardApplicationID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Approvals", "CardApplicationID", c => c.Int(nullable: false));
            DropColumn("dbo.Approvals", "CardApplicationIDs");
            CreateIndex("dbo.Approvals", "CardApplicationID");
            AddForeignKey("dbo.Approvals", "CardApplicationID", "dbo.CardApplications", "ID", cascadeDelete: true);
        }
    }
}
