namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewModels11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "InstitutionID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "InstitutionID");
        }
    }
}
