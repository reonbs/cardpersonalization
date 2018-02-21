namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModel21 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuditRecords", "Module", c => c.String());
            AddColumn("dbo.AuditRecords", "Action", c => c.String());
            AddColumn("dbo.AuditRecords", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AuditRecords", "Description");
            DropColumn("dbo.AuditRecords", "Action");
            DropColumn("dbo.AuditRecords", "Module");
        }
    }
}
