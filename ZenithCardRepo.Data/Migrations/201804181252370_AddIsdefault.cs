namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsdefault : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsDefaultPassword", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsDefaultPassword");
        }
    }
}
