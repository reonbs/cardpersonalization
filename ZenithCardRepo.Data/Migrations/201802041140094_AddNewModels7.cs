namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewModels7 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ImageValidationSettings", "Width", c => c.Int(nullable: false));
            AlterColumn("dbo.ImageValidationSettings", "Height", c => c.Int(nullable: false));
            AlterColumn("dbo.ImageValidationSettings", "HeadTilt", c => c.Int(nullable: false));
            AlterColumn("dbo.ImageValidationSettings", "ImageSize", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ImageValidationSettings", "ImageSize", c => c.String());
            AlterColumn("dbo.ImageValidationSettings", "HeadTilt", c => c.String());
            AlterColumn("dbo.ImageValidationSettings", "Height", c => c.String());
            AlterColumn("dbo.ImageValidationSettings", "Width", c => c.String());
        }
    }
}
