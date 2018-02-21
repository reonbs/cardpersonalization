namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangesToContext : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.NationalityCodeDesigns", newName: "NationalityCodes");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.NationalityCodes", newName: "NationalityCodeDesigns");
        }
    }
}
