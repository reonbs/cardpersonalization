namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModel14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CardApplications", "CreatedBy", c => c.String());
            AddColumn("dbo.CardApplications", "ModifiedBy", c => c.String());
            AddColumn("dbo.CardApplications", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.CardApplications", "DateModified", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CardApplications", "DateModified");
            DropColumn("dbo.CardApplications", "DateCreated");
            DropColumn("dbo.CardApplications", "ModifiedBy");
            DropColumn("dbo.CardApplications", "CreatedBy");
        }
    }
}
