namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newmodels4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "RegistrationType", c => c.String());
            AddColumn("dbo.AspNetUsers", "OrganizationCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "OrganizationCode");
            DropColumn("dbo.AspNetUsers", "RegistrationType");
        }
    }
}
