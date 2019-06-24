namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModel23 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuditRecords", "InstitutionID", c => c.Int(nullable: false));
            CreateIndex("dbo.AuditRecords", "InstitutionID");
            AddForeignKey("dbo.AuditRecords", "InstitutionID", "dbo.Institutions", "ID", cascadeDelete: true);
            DropColumn("dbo.AuditRecords", "Institution");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AuditRecords", "Institution", c => c.String());
            DropForeignKey("dbo.AuditRecords", "InstitutionID", "dbo.Institutions");
            DropIndex("dbo.AuditRecords", new[] { "InstitutionID" });
            DropColumn("dbo.AuditRecords", "InstitutionID");
        }
    }
}
