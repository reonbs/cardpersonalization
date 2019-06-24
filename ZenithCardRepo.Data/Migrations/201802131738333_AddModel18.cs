namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModel18 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CardApplications", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CardApplications", "IsDeleted");
        }
    }
}
