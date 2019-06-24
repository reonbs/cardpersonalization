namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangesToClassName : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.CityLegends", newName: "Cities");
            RenameTable(name: "dbo.MaritalStatusLegends", newName: "MaritalStatus");
            RenameTable(name: "dbo.StateLegends", newName: "States");
            CreateTable(
                "dbo.Sexes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            DropTable("dbo.SexLegends");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SexLegends",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            DropTable("dbo.Sexes");
            RenameTable(name: "dbo.States", newName: "StateLegends");
            RenameTable(name: "dbo.MaritalStatus", newName: "MaritalStatusLegends");
            RenameTable(name: "dbo.Cities", newName: "CityLegends");
        }
    }
}
