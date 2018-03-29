namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmodel344 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Approvals",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Approver = c.String(),
                        Comment = c.String(),
                        CardApplicationID = c.Int(nullable: false),
                        Rank = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.CardApplications", t => t.CardApplicationID, cascadeDelete: true)
                .Index(t => t.CardApplicationID);
            
            AddColumn("dbo.CardApplications", "IsApproved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Approvals", "CardApplicationID", "dbo.CardApplications");
            DropIndex("dbo.Approvals", new[] { "CardApplicationID" });
            DropColumn("dbo.CardApplications", "IsApproved");
            DropTable("dbo.Approvals");
        }
    }
}
