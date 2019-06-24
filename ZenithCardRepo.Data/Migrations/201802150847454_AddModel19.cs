namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModel19 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditRecords",
                c => new
                    {
                        AuditRecordID = c.Guid(nullable: false),
                        UserName = c.String(),
                        IPAddress = c.String(),
                        AreaAccessed = c.String(),
                        TimeAccessed = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AuditRecordID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AuditRecords");
        }
    }
}
