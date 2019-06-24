namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModel22 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuditRecords", "Institution", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AuditRecords", "Institution");
        }
    }
}
