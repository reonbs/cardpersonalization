namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModel17 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CardApplications", "InstitutionID", c => c.Int(nullable: false));
            CreateIndex("dbo.CardApplications", "InstitutionID");
            AddForeignKey("dbo.CardApplications", "InstitutionID", "dbo.Institutions", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CardApplications", "InstitutionID", "dbo.Institutions");
            DropIndex("dbo.CardApplications", new[] { "InstitutionID" });
            DropColumn("dbo.CardApplications", "InstitutionID");
        }
    }
}
