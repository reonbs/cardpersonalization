namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModel15 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CardApplications", "DateModified", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CardApplications", "DateModified", c => c.DateTime(nullable: false));
        }
    }
}
