namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addmodel33 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RolePermissions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RoleID = c.Int(nullable: false),
                        PermissionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RolePermissions");
        }
    }
}
