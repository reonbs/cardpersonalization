namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewModels6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ImageValidationSettings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Width = c.String(),
                        Height = c.String(),
                        HeadTilt = c.String(),
                        ImageSize = c.String(),
                        ImageFormat = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ImageValidationSettings");
        }
    }
}
