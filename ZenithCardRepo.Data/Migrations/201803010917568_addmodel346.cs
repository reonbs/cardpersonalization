namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmodel346 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Approvals", "Rank", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Approvals", "Rank", c => c.String());
        }
    }
}
