namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewModels10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        InstitutionID = c.Int(nullable: false),
                        Code = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Institutions", t => t.InstitutionID, cascadeDelete: true)
                .Index(t => t.InstitutionID);
            
            CreateTable(
                "dbo.Institutions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Code = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Departments", "InstitutionID", "dbo.Institutions");
            DropIndex("dbo.Departments", new[] { "InstitutionID" });
            DropTable("dbo.Institutions");
            DropTable("dbo.Departments");
        }
    }
}
