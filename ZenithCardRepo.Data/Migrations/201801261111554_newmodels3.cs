namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newmodels3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OrganizationProfiles", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.OrganizationProfiles", "Code", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrganizationProfiles", "Code", c => c.String());
            AlterColumn("dbo.OrganizationProfiles", "Name", c => c.String());
        }
    }
}
