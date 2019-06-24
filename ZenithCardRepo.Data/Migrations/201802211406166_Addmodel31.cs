namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addmodel31 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CardApplications", "NameonCard", c => c.String(nullable: false, maxLength: 21));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CardApplications", "NameonCard", c => c.String(nullable: false, maxLength: 23));
        }
    }
}
