namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddModel20 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuditRecords", "Data", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.AuditRecords", "Data");
        }
    }
}
