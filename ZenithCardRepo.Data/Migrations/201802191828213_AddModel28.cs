namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModel28 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsDisabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsDisabled");
        }
    }
}
