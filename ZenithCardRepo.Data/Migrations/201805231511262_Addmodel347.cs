namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addmodel347 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CardApplications", "MainAccountNo", c => c.String(nullable: false, maxLength: 10));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CardApplications", "MainAccountNo", c => c.String(maxLength: 10));
        }
    }
}
