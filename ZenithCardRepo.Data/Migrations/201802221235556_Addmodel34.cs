namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addmodel34 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RolePermissions", "RoleID", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RolePermissions", "RoleID", c => c.Int(nullable: false));
        }
    }
}
