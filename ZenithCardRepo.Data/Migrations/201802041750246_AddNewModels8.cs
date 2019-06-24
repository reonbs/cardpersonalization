namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewModels8 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.CardApplications", "ReferenceNo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CardApplications", "ReferenceNo", c => c.String());
        }
    }
}
