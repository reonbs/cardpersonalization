namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newmodels2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrganizationProfiles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Code = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OrganizationProfiles");
        }
    }
}
