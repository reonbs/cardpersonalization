namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModel27 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String());
            AddColumn("dbo.AspNetUsers", "MiddleName", c => c.String());
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String());
            AddColumn("dbo.AspNetUsers", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "DateModified", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "CreatedBy", c => c.String());
            AddColumn("dbo.AspNetUsers", "ModifiedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ModifiedBy");
            DropColumn("dbo.AspNetUsers", "CreatedBy");
            DropColumn("dbo.AspNetUsers", "DateModified");
            DropColumn("dbo.AspNetUsers", "DateCreated");
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetUsers", "MiddleName");
            DropColumn("dbo.AspNetUsers", "FirstName");
        }
    }
}
