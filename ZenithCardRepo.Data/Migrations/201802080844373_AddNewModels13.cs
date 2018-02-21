namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewModels13 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "RegistrationType");
            DropColumn("dbo.AspNetUsers", "OrganizationCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "OrganizationCode", c => c.String());
            AddColumn("dbo.AspNetUsers", "RegistrationType", c => c.String());
        }
    }
}
